using System;
using Cysharp.Threading.Tasks;
using Game.CodeBase.Model;
using Game.CodeBase.Player;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;
using Game.CodeBase.Shared;
using Game.CodeBase.UI;
using Mirror;
using UniRx;
using UnityEngine;

namespace Game.CodeBase.Game.Lobby
{
    public class Lobby : NetworkBehaviour
    {
        public static Action<Lobby> Spawned;
        public IReactiveCommand<LobbyPlayer> AddPlayer = new ReactiveCommand<LobbyPlayer>();
        public IConnectableObservable<LobbyPlayer> AddPlayerBuffered;

        private readonly SyncList<LobbyPlayer> _players = new SyncList<LobbyPlayer>();
        private readonly SyncList<uint> _netIds = new SyncList<uint>();

        private LobbyUI _lobbyUI;
        private LobbyPlayer _localClient;
        private LobbyFactory _factory;
        private PlayerProgressData _playerProgressData;
        private NetworkSpawner _spawner;

        private void Awake()
        {
            _players.Callback += OnPlayersChanged;
            AddPlayerBuffered = AddPlayer.Replay(4);
            AddPlayerBuffered.Connect();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Spawned?.Invoke(this);
        }

        public void Initialize(LobbyFactory factory, LobbyUI lobbyUI, PlayerProgressData playerProgressData, NetworkSpawner spawner)
        {
            _spawner = spawner;
            _playerProgressData = playerProgressData;
            _factory = factory;
            _lobbyUI = lobbyUI;
            AddPlayerBuffered.Subscribe(SetupPlayer);

            if (isServer)
            {
                _lobbyUI.SetHostButtons();
            }
            else
            {
                foreach (var id in _netIds)
                    StartCoroutine(GetPlayerAsync(id).ToCoroutine());
            }
        }

        public async void CreatePlayerAsync(BasePlayer player)
        {
            // var client = player.GetComponent<NetworkIdentity>();
            Debug.Log($"Spawn LobbyPlayer for id={player.Id}");
            bool isHost = player.hasAuthority;

            LobbyPlayer lobbyPlayer = await _factory.CreatePlayer(player, isHost);
            _players.Add(lobbyPlayer);
            _netIds.Add(lobbyPlayer.netId);
        }

        private void SetupPlayer(LobbyPlayer player)
        {
            Debug.Log("Setup player", player);
            if (player == null) return;

            if (player.hasAuthority)
            {
                _localClient = player;
                _lobbyUI.Username.Subscribe(StoreUsername);
                _lobbyUI.Username.Subscribe(player.SetUsername);
                _lobbyUI.Ready.Subscribe(player.SetReady);
                _lobbyUI.Color.Subscribe(player.SetColor);
            }

            _lobbyUI.SetupSlot(player);
        }

        private async UniTask GetPlayerAsync(uint id)
        {
            var lobbyPlayer = await _spawner.AwaitForNetworkEntity<LobbyPlayer>(id, TimeSpan.FromSeconds(3));
            if (lobbyPlayer != null)
                AddPlayer.Execute(lobbyPlayer);
            else
                Debug.LogError($"LobbyPlayer (netId={id}) could not be loaded");
        }

        private void StoreUsername(string value)
        {
            PlayerProgress progress = _playerProgressData.Progress;
            progress.Username = value;

            _playerProgressData.SetPlayerProgress(progress);
        }

        private void OnPlayersChanged(SyncList<LobbyPlayer>.Operation op, int _, LobbyPlayer __, LobbyPlayer player)
        {
            if (op == SyncList<LobbyPlayer>.Operation.OP_ADD)
            {
                AddPlayer.Execute(player);
            }
        }
    }
}