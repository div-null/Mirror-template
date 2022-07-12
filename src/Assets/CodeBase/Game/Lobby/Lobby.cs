using System;
using Cysharp.Threading.Tasks;
using Game.CodeBase.Player;
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

        private SyncList<LobbyPlayer> _players = new SyncList<LobbyPlayer>();
        private SyncList<uint> _netIds = new SyncList<uint>();

        private LobbyUI _lobbyUI;
        private LobbyPlayer localClient;
        private LobbyFactory _factory;

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
            // foreach (var lobbyPlayer in _players)
            //     AddPlayer.Execute(lobbyPlayer);

            foreach (var id in _netIds) 
                StartCoroutine(AwaitLobbyPlayer(id).ToCoroutine());
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

        public void Initialize(LobbyFactory factory, LobbyUI lobbyUI)
        {
            _factory = factory;
            _lobbyUI = lobbyUI;
            AddPlayerBuffered.Subscribe(SetupPlayer);
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
            bool isOwner = HasAuthority(player);
            localClient ??= isOwner ? player : null;

            _lobbyUI.SetupSlot(player, isOwner);
        }

        private void OnPlayersChanged(SyncList<LobbyPlayer>.Operation op, int _, LobbyPlayer __, LobbyPlayer player)
        {
            if (op == SyncList<LobbyPlayer>.Operation.OP_ADD)
            {
                AddPlayer.Execute(player);
            }
        }

        private bool HasAuthority(NetworkBehaviour entity) =>
            connectionToServer?.connectionId == entity.connectionToServer?.connectionId ||
            connectionToClient?.connectionId == entity.connectionToClient?.connectionId;
    }
}