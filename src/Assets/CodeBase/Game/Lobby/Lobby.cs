﻿using System;
using System.Threading.Tasks;
using Game.CodeBase.Player;
using Game.CodeBase.UI;
using Mirror;

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
            _players.Callback += onPlayersChanged;
        }

        public void AddPlayer(LobbyPlayer player)
        {
            bool isOwner = HasAuthority(player);
            localClient ??= isOwner ? player : null;
            
            _lobbyUI.SetupSlot(player, isOwner);
        }

        private bool HasAuthority(LobbyPlayer player) =>
            connectionToServer == player.connectionToServer;

        [Command]
        public void CmdCreatePlayer(BasePlayer player, NetworkConnectionToClient conn = null)
        {
            createPlayerAsync(player, conn);
        }

        private void onPlayersChanged(SyncList<LobbyPlayer>.Operation op, int _, LobbyPlayer __, LobbyPlayer player)
        {
            if (op == SyncList<LobbyPlayer>.Operation.OP_ADD)
            {
                AddPlayer(player);
            }
        }

        private async Task createPlayerAsync(BasePlayer player, NetworkConnection conn)
        {
            bool isHost = this.connectionToServer == conn;
            LobbyPlayer lobbyPlayer = await _factory.CreatePlayer(conn, player, isHost);
            _players.Add(lobbyPlayer);
            // AddPlayer(lobbyPlayer);
        }
    }
}