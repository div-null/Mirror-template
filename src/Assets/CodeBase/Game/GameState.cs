using Mirror;
using UniRx;
using UnityEngine;
using Game.CodeBase.Player;
using Game.CodeBase.Services.Network;
using Game.CodeBase.Shared;

namespace Game.CodeBase.Game
{
    public class GameState
    {
        public readonly ReactiveCommand<BasePlayer> AddPlayer = new();
        public readonly IConnectableObservable<BasePlayer> AddPlayerBuffered;

        private readonly PlayerFactory _playerFactory;
        private readonly CustomNetworkManager _networkManager;
        public BasePlayer[] Players { get; } = new BasePlayer[Constants.MaxPlayers];


        public string ServerName { get; private set; } = "Server";
        public int MinReadyPlayers { get; private set; } = 2;
        public int MaxPlayers { get; private set; } = Constants.MaxPlayers;

        public bool ConnectToAvailableServerAutomatically { get; set; } = true;

        public GameState(CustomNetworkManager networkManager, PlayerFactory playerFactory)
        {
            _networkManager = networkManager;
            _playerFactory = playerFactory;
            _networkManager.OnSpawnClient += OnSpawnPlayer;
            AddPlayerBuffered = AddPlayer.Replay(4);
            AddPlayerBuffered.Connect();
        }

        private void OnSpawnPlayer(NetworkConnectionToClient conn)
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


        private int GetAvailableId()
        {
            for (int i = 0; i < 4; i++)
            {
                if (Players[i] == null)
                    return i;
            }


            Debug.Log("Its not empty!");
            return -1;
        }
    }
}