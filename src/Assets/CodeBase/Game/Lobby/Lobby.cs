using System;
using CodeBase.Model;
using Cysharp.Threading.Tasks;
using Game.CodeBase.Player;
using Game.CodeBase.Services;
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

            foreach (var id in _netIds)
                StartCoroutine(AwaitLobbyPlayer(id).ToCoroutine());
        }

        public void Initialize(LobbyFactory factory, LobbyUI lobbyUI, PlayerProgressData playerProgressData)
        {
            _playerProgressData = playerProgressData;
            _factory = factory;
            _lobbyUI = lobbyUI;
            AddPlayerBuffered.Subscribe(SetupPlayer);

            if (isServer)
                _lobbyUI.SetHostButtons();
        }

        public async void CreatePlayerAsync(BasePlayer player, NetworkConnectionToClient conn = null)
        {
            Debug.Log($"Spawn LobbyPlayer for id={player.Id}");
            bool isHost = HasAuthority(player);
            
            LobbyPlayer lobbyPlayer = await _factory.CreatePlayer(conn, player, isHost);
            _players.Add(lobbyPlayer);
            _netIds.Add(lobbyPlayer.netId);
        }

        private void SetupPlayer(LobbyPlayer player)
        {
            Debug.Log("Setup player", player);
            if (player == null) return;

            if (HasAuthority(player))
            {
                _localClient = player;
                _lobbyUI.Username.Subscribe(StoreUsername);
                _lobbyUI.Username.Subscribe(player.SetUsername);
                _lobbyUI.Ready.Subscribe(player.SetReady);
                _lobbyUI.Color.Subscribe(player.SetColor);
            }

            _lobbyUI.SetupSlot(player);
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

        private async UniTask AwaitLobbyPlayer(uint id)
        {
            Debug.Log($"Awaiting for player netid={id}");
            while (true)
            {
                await UniTask.NextFrame();
                if (NetworkClient.spawned.TryGetValue(id, out NetworkIdentity identity))
                {
                    var lobbyPlayer = identity.gameObject.GetComponent<LobbyPlayer>();
                    AddPlayer.Execute(lobbyPlayer);
                    break;
                }
            }
        }

        private bool HasAuthority(NetworkBehaviour entity) =>
            connectionToServer?.connectionId == entity.connectionToServer?.connectionId ||
            connectionToClient?.connectionId == entity.connectionToClient?.connectionId;
    }
}