using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Model;
using Game.CodeBase.Game;
using JetBrains.Annotations;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

namespace Game.CodeBase.Services.Network
{
    [RequireComponent(typeof(CustomNetworkManager))]
    public class ClientAuthenticator : NetworkAuthenticator
    {
        private readonly HashSet<NetworkConnection> _connectionsPendingDisconnect = new HashSet<NetworkConnection>();
        private readonly NetworkConnection[] _connectedClients = new NetworkConnection[Constants.MaxPlayers];

        private PlayerProgressData _progressService;
        private CustomNetworkManager _networkManager;


        [Inject]
        public void Initialize(CustomNetworkManager networkManager, PlayerProgressData progressData)
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

            int nameDuplicates = _networkManager.Players
                .NotNull()
                .Count(player => player.Username.StartsWith(msg.Username));

            var playerProgress = new PlayerProgress(DuplicateName(msg.Username, nameDuplicates), msg.Color);

            // This will be read in Player.OnStartServer
            conn.authenticationData = playerProgress;

            AuthResponseMessage authResponseMessage = Respond();

            conn.Send(authResponseMessage);
            ServerAccept(conn);
        }

        private static AuthResponseMessage Respond([CanBeNull] string updatedName = null)
        {
            return updatedName != null
                ? AuthResponseMessage.FixedName(updatedName)
                : AuthResponseMessage.Success();
        }

        private static string DuplicateName(string name, int nameDuplicates) =>
            nameDuplicates > 0 ? name + $" ({nameDuplicates})" : name;


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
            PlayerProgress progress = _progressService.Progress;
            AuthRequestMessage authRequestMessage = new AuthRequestMessage
            {
                Username = progress.Username,
                Color = progress.ColorData.Color
            };

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