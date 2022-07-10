using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.CodeBase.Game.Lobby;
using Game.CodeBase.Player;
using Game.CodeBase.Services.Network;
using kcp2k;
using Mirror;
using UniRx;
using UnityEngine;

namespace Game.CodeBase.Infrastructure.States
{
    public class HostLobbyState : IPayloadedState<ushort>
    {
        private readonly LobbyFactory _lobbyFactory;
        private readonly ServersObserver _serversObserver;
        private readonly AdvancedNetworkManager _networkManager;
        private Lobby _lobby;
        private CompositeDisposable _disposable;

        public HostLobbyState(LobbyFactory lobbyFactory, ServersObserver serversObserver, AdvancedNetworkManager networkManager)
        {
            _networkManager = networkManager;
            _serversObserver = serversObserver;
            _lobbyFactory = lobbyFactory;
        }

        public async void Enter(ushort server)
        {
            _disposable = new CompositeDisposable();
            _networkManager.gameObject.GetComponent<KcpTransport>().Port = server;

            var replay = _networkManager.AddPlayer.Replay(4);
            replay.Connect().AddTo(_disposable);
            replay.Subscribe(OnAddPlayer).AddTo(_disposable);

            _networkManager.StartHost();
            await InitializeLobby();

            _serversObserver.AdvertiseServer();
        }

        private void OnAddPlayer(BasePlayer player)
        {
            Debug.Log($"Create LobbyPlayer for id={player.Id}");
            _lobby.CmdCreatePlayer(player);
        }

        public void Exit()
        {
            _disposable?.Dispose();
            // NetworkServer.Destroy(_lobby.gameObject);
        }

        private async Task InitializeLobby()
        {
            var lobbyUI = await _lobbyFactory.CreateUI();
            _lobby = await _lobbyFactory.SpawnLobby();
            _lobby.Initialize(_lobbyFactory, lobbyUI);
        }
    }
}