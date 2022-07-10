using System;
using System.Collections.Generic;
using System.Linq;
using Game.CodeBase.Game;
using Game.CodeBase.Infrastructure;
using Game.CodeBase.Player;
using Mirror;
using UniRx;
using UnityEngine;
using VContainer;

namespace Game.CodeBase.Services.Network
{
    public class AdvancedNetworkManager : NetworkManager, ICoroutineRunner
    {
        public readonly ReactiveCommand<BasePlayer> AddPlayer = new ReactiveCommand<BasePlayer>();

        [Header("Base information")] public int MinConnections = 2;
        [Scene] public string CurrentScene = string.Empty;

        public bool ConnectToAvailableServerAutomatically = false;

        public BasePlayer[] Players { get; } = new BasePlayer[4];
        public List<string> PlayerNames = new List<string>();

        private PlayerFactory _playerFactory;

        public event Action<NetworkConnection> OnClientConnected;
        public event Action<NetworkConnection> OnClientDisconnected;
        public event Action<NetworkConnectionToClient> OnClientConnectedOnServerSide;
        public event Action<NetworkConnectionToClient> OnClientDisconnectedOnServerSide;
        public event Action<NetworkConnectionToClient> OnServerAddedPlayer;


        [Inject]
        public void Initialize(PlayerFactory playerFactory)
        {
            _playerFactory = playerFactory;
        }

        public override void OnStartServer()
        {
            spawnPrefabs = Resources.LoadAll<GameObject>("NetworkPrefabs").ToList();
        }

        public override void OnStartClient()
        {
            var spawnableObjects = Resources.LoadAll<GameObject>("NetworkPrefabs");

            foreach (var obj in spawnableObjects)
            {
                NetworkClient.RegisterPrefab(obj);
            }
        }

        public override void OnStopServer()
        {
            foreach (var player in Players)
            {
                //if (player != null) player.Destroy();
            }
        }

        /// <summary>
        /// Called on the client when connected to a server.
        /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            OnClientConnected?.Invoke(conn);
        }

        /// <summary>
        /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
        /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            int availableId = GetAvailableId();
            if (availableId != -1)
            {
                BasePlayer player = _playerFactory.CreatePlayer(conn, availableId);
                Players[availableId] = player;

                NetworkServer.AddPlayerForConnection(conn, player.gameObject);
                AddPlayer.Execute(player);
            }
        }

        /// <summary>
        /// Called on clients when disconnected from a server.
        /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
        /// </summary>
        /// <param name="conn">Connection to the server.</param>
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            var player = conn.identity.GetComponent<BasePlayer>();
            PlayerNames.Remove(player.Username);

            OnClientDisconnected?.Invoke(conn);
        }

        /// <summary>
        /// Called on the server when a new client connects.
        /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (numPlayers > maxConnections)
            {
                conn.Disconnect();
            }

            //TODO: Сделать возможность проверки на повторяющиеся ники + реконнект и пауза для ожидания реконнекта
            OnClientConnectedOnServerSide?.Invoke(conn);
        }

        /// <summary>
        /// Called on the server when a client disconnects.
        /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            OnClientDisconnectedOnServerSide?.Invoke(conn);
            base.OnServerDisconnect(conn);
        }

        public int GetAvailableId()
        {
            for (int i = 0; i < 4; i++)
            {
                if (Players[i] == null)
                    return i;
            }


            Debug.Log("Its not empty!");
            return -1;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
        }

        #region Scene management

        /// <summary>
        /// This causes the server to switch scenes and sets the networkSceneName.
        /// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready. The clients must call NetworkClient.Ready() again to participate in the new scene.</para>
        /// </summary>
        /// <param name="newSceneName"></param>
        public override void ServerChangeScene(string newSceneName)
        {
            base.ServerChangeScene(newSceneName);
        }


        /// <summary>
        /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
        /// <para>This allows server to do work / cleanup / prep before the scene changes.</para>
        /// </summary>
        /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
        public override void OnServerChangeScene(string newSceneName)
        {
            base.OnServerChangeScene(newSceneName);
        }

        /// <summary>
        /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
        /// </summary>
        /// <param name="sceneName">The name of the new scene.</param>
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
        }

        /// <summary>
        /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
        /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
        /// </summary>
        /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
        /// <param name="sceneOperation">Scene operation that's about to happen</param>
        /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        }

        /// <summary>
        /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
        /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
        /// </summary>
        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();
        }

        #endregion

        #region Server System Callbacks

        /// <summary>
        /// Called on the server when a client is ready.
        /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
        }

        /// <summary>
        /// Called on the server when a network error occurs for a client connection.
        /// </summary>
        /// <param name="conn">Connection from client.</param>
        /// <param name="exception">Error</param>
        public override void OnServerError(NetworkConnectionToClient conn, Exception exception)
        {
            base.OnServerError(conn, exception);
        }

        #endregion

        #region Client System Callbacks

        /// <summary>
        /// Called on clients when a network error occurs.
        /// </summary>
        /// <param name="exception">Error</param>
        public override void OnClientError(Exception exception)
        {
            base.OnClientError(exception);
        }

        /// <summary>
        /// Called on clients when a servers tells the client it is no longer ready.
        /// <para>This is commonly used when switching scenes.</para>
        /// </summary>
        public override void OnClientNotReady()
        {
            base.OnClientNotReady();
        }

        #endregion
    }
}