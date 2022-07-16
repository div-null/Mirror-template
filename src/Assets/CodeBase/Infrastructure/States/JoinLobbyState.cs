using System;
using Cysharp.Threading.Tasks;
using Game.CodeBase.Game.Lobby;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;
using Game.CodeBase.UI;
using UniRx;

namespace Game.CodeBase.Infrastructure.States
{
    public class JoinLobbyState : IPayloadedState<ServerInfo>
    {
        private readonly CustomNetworkManager _networkManager;
        private readonly LobbyFactory _lobbyFactory;
        private readonly PlayerProgressData _playerProgressData;
        
        private Lobby _lobby;
        private LobbyUI _lobbyUI;
        private IObservable<Lobby> _networkingSynced;
        private NetworkSpawner _spawner;

        public JoinLobbyState(LobbyFactory lobbyFactory, CustomNetworkManager networkManager, PlayerProgressData playerProgressData, NetworkSpawner spawner)
        {
            _spawner = spawner;
            _playerProgressData = playerProgressData;
            _lobbyFactory = lobbyFactory;
            _networkManager = networkManager;
        }

        public async void Enter(ServerInfo server)
        {
            _networkingSynced = Observable.FromEvent<Lobby>(
                    ev => Lobby.Spawned += ev,
                    ev => Lobby.Spawned -= ev)
                .First();

            _networkingSynced.Subscribe(LobbySpawned);
            _networkManager.StartClient(server.Uri);
            await UniTask.WaitUntil(() => _networkManager.isNetworkActive);
        }

        public void Exit()
        {
        }


        private async void LobbySpawned(Lobby lobby)
        {
            _lobbyUI = await _lobbyFactory.CreateUI();
            _lobby = lobby;
            _lobby.Initialize(_lobbyFactory, _lobbyUI, _playerProgressData, _spawner);
        }
    }
}