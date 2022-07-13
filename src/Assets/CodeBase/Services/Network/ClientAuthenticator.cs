﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.CodeBase.Shared;
using Mirror;
using UnityEngine;
using VContainer;

namespace Game.CodeBase.Services.Network
{
    [RequireComponent(typeof(CustomNetworkManager))]
    public class ClientAuthenticator : NetworkAuthenticator
    {
        private readonly HashSet<NetworkConnection> _connectionsPendingDisconnect = new HashSet<NetworkConnection>();
        private readonly NetworkConnection[] _connectedClients = new NetworkConnection[Constants.MaxPlayers];

        private IAuthRequestHandler[] _requestHandlers;
        private IAuthRequestProvider _requestProvider;

        [Inject]
        public void Initialize(IAuthRequestProvider requestProvider, IEnumerable<IAuthRequestHandler> requestHandlers)
        {
            _requestProvider = requestProvider;
            _requestHandlers = requestHandlers.ToArray();
        }

        #region Messages

        public struct AuthRequestMessage : NetworkMessage
        {
            public string Username;
            public Color Color;
        }

        public struct AuthResponseMessage : NetworkMessage
        {
            public byte Code;
            public string Message;

            public static AuthResponseMessage Success() => new()
            {
                Code = 100,
                Message = "Success"
            };

            public static AuthResponseMessage FixedName(string updatedName) => new()
            {
                Code = 101,
                Message = updatedName
            };
        }

        #endregion

        #region Server

        /// <summary>
        /// Called on server from StartServer to initialize the Authenticator
        /// <para>Server message handlers should be registered in this method.</para>
        /// </summary>
        public override void OnStartServer()
        {
            // register a handler for the authentication request we expect from client
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

        /// <summary>
        /// Called on server from StopServer to reset the Authenticator
        /// <para>Server message handlers should be registered in this method.</para>
        /// </summary>
        public override void OnStopServer()
        {
            // unregister the handler for the authentication request
            NetworkServer.UnregisterHandler<AuthRequestMessage>();
        }

        /// <summary>
        /// Called on server from OnServerAuthenticateInternal when a client needs to authenticate
        /// </summary>
        /// <param name="conn">Connection to client.</param>
        public override void OnServerAuthenticate(NetworkConnectionToClient conn)
        {
            // do nothing...wait for AuthRequestMessage from client
        }

        /// <summary>
        /// Called on server when the client's AuthRequestMessage arrives
        /// </summary>
        /// <param name="conn">Connection to client.</param>
        /// <param name="msg">The message payload</param>
        public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            Debug.Log($"Authentication Request: {msg.Username}");

            if (_connectionsPendingDisconnect.Contains(conn)) return;

            foreach (var handler in _requestHandlers)
            {
                if (!handler.Accept(conn, msg, out AuthResponseMessage response))
                {
                    conn.Send(response);
                    StartCoroutine(DelayedDisconnect(conn, 1));
                    return;
                }
            }

            conn.Send(AuthResponseMessage.Success());
            ServerAccept(conn);
        }


        IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            ServerReject(conn);

            yield return null;
            _connectionsPendingDisconnect.Remove(conn);
        }

        #endregion

        #region Client

        /// <summary>
        /// Called on client from StartClient to initialize the Authenticator
        /// <para>Client message handlers should be registered in this method.</para>
        /// </summary>
        public override void OnStartClient()
        {
            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }

        /// <summary>
        /// Called on client from StopClient to reset the Authenticator
        /// <para>Client message handlers should be unregistered in this method.</para>
        /// </summary>
        public override void OnStopClient()
        {
            // unregister the handler for the authentication response
            NetworkClient.UnregisterHandler<AuthResponseMessage>();
        }

        /// <summary>
        /// Called on client from OnClientAuthenticateInternal when a client needs to authenticate
        /// </summary>
        public override void OnClientAuthenticate()
        {
            AuthRequestMessage authRequestMessage = _requestProvider.Request();
            NetworkClient.connection.Send(authRequestMessage);
        }

        /// <summary>
        /// Called on client when the server's AuthResponseMessage arrives
        /// </summary>
        /// <param name="msg">The message payload</param>
        public void OnAuthResponseMessage(AuthResponseMessage msg)
        {
            switch (msg.Code)
            {
                case 100:
                    Debug.Log($"Authentication Response: Success");
                    ClientAccept();
                    break;
                case 101:
                    Debug.Log($"Authentication Response: Fixed name");
                    ClientAccept();
                    break;
                default:
                    Debug.LogError($"Authentication Response: {msg.Message}");

                    NetworkManager.singleton.StopHost();

                    Debug.LogError(msg.Message);
                    break;
            }
        }

        #endregion
    }
}