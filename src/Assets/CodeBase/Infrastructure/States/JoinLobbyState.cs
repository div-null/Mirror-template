using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.CodeBase.Game.Lobby;
using Game.CodeBase.Services.Network;
using Mirror;
using UnityEngine;

namespace Game.CodeBase.Infrastructure.States
{
    public class JoinLobbyState : IPayloadedState<ServerInfo>
    {
        private readonly NetworkManager _networkManager;
        private readonly LobbyFactory _lobbyFactory;
        private Lobby _lobby;

        public JoinLobbyState(LobbyFactory lobbyFactory, NetworkManager networkManager)
        {
            _lobbyFactory = lobbyFactory;
            _networkManager = networkManager;
        }

        public async void Enter(ServerInfo server)
        {
            _networkManager.StartClient(server.Uri);
            await UniTask.WaitUntil(() => _networkManager.isNetworkActive);
            await InitializeLobby();
        }

        public void Exit()
        {
        }

        private async Task InitializeLobby()
        {
            var lobbyUI = await _lobbyFactory.CreateUI();
            _lobby = Object.FindObjectOfType<Lobby>();
            _lobby.Initialize(_lobbyFactory, lobbyUI);
        }
    }
}