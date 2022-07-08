using System.Collections;
using System.Collections.Generic;
using Game.CodeBase.Data;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using VContainer;

namespace Game.CodeBase.Services.Network
{
    public class ClientAuthenticator : NetworkAuthenticator
    {
        readonly HashSet<NetworkConnection> connectionsPendingDisconnect = new HashSet<NetworkConnection>();
        private readonly NetworkConnection[] connectedClients = new NetworkConnection[4];

        public PlayerProgressData _progressService;
        private AdvancedNetworkManager _networkManager;


        [Inject]
        public void Initialize(AdvancedNetworkManager networkManager, PlayerProgressData progressData)
        {
            _networkManager = networkManager;
            _progressService = progressData;
        }

        #region Messages

        public struct AuthRequestMessage : NetworkMessage
        {
            public string Username;
            public Color Color;
        }

        public struct AuthResponseMessage : NetworkMessage
        {
            public byte code;
            public string message;
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

            if (connectionsPendingDisconnect.Contains(conn)) return;

            if (!_networkManager.PlayerNames.Contains(msg.Username))
            {
                _networkManager.PlayerNames.Add(msg.Username);

                // This will be read in Player.OnStartServer
                conn.authenticationData = new PlayerProgress(msg.Username, msg.Color);

                // create and send msg to client so it knows to proceed
                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 100,
                    message = "Success"
                };

                conn.Send(authResponseMessage);
                ServerAccept(conn);
            }
            else
            {
                connectionsPendingDisconnect.Add(conn);

                AuthResponseMessage authResponseMessage = new AuthResponseMessage
                {
                    code = 200,
                    message = "Username already in use...try again"
                };

                conn.Send(authResponseMessage);
                conn.isAuthenticated = false;
                StartCoroutine(DelayedDisconnect(conn, 1f));
            }
        }


        IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            ServerReject(conn);

            yield return null;
            connectionsPendingDisconnect.Remove(conn);
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
            PlayerProgress progress = _progressService.Progress;
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                Username = progress.Username,
                Color = progress.ColorData
            };

            NetworkClient.connection.Send(authRequestMessage);
        }

        /// <summary>
        /// Called on client when the server's AuthResponseMessage arrives
        /// </summary>
        /// <param name="msg">The message payload</param>
        public void OnAuthResponseMessage(AuthResponseMessage msg)
        {
            if (msg.code == 100)
            {
                Debug.Log($"Authentication Response: {msg.message}");

                // Authentication has been accepted
                ClientAccept();
            }
            else
            {
                Debug.LogError($"Authentication Response: {msg.message}");

                NetworkManager.singleton.StopHost();

                Debug.LogError(msg.message);
            }
        }

        #endregion
    }
}