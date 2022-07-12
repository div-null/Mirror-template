using Game.CodeBase.Player;
using Game.CodeBase.UI;
using Mirror;
using UnityEngine;

namespace Game.CodeBase.Game.Lobby
{
    public class Lobby : NetworkBehaviour
    {
        private SyncList<LobbyPlayer> _players = new SyncList<LobbyPlayer>();
        private LobbyUI _lobbyUI;
        private LobbyPlayer localClient;
        private LobbyFactory _factory;

        public void Initialize(LobbyFactory factory, LobbyUI lobbyUI)
        {
            _factory = factory;
            _lobbyUI = lobbyUI;
            _players.Callback += OnPlayersChanged;
        }

        public void AddPlayer(LobbyPlayer player)
        {
            bool isOwner = HasAuthority(player);
            localClient ??= isOwner ? player : null;

            _lobbyUI.SetupSlot(player, isOwner);
        }

        public async void CreatePlayerAsync(BasePlayer player, NetworkConnectionToClient conn = null)
        {
            Debug.Log($"Spawn LobbyPlayer for id={player.Id}");
            bool isHost = HasAuthority(player);
            LobbyPlayer lobbyPlayer = await _factory.CreatePlayer(conn, player, isHost);
            _players.Add(lobbyPlayer);
        }

        private void OnPlayersChanged(SyncList<LobbyPlayer>.Operation op, int _, LobbyPlayer __, LobbyPlayer player)
        {
            if (op == SyncList<LobbyPlayer>.Operation.OP_ADD)
            {
                AddPlayer(player);
            }
        }

        private bool HasAuthority(NetworkBehaviour entity) =>
            connectionToServer?.connectionId == entity.connectionToServer?.connectionId ||
            connectionToClient?.connectionId == entity.connectionToClient?.connectionId;
    }
}