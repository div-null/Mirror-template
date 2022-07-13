using System.Collections.Generic;
using System.Linq;
using Game.CodeBase.Infrastructure.States.CodeBase.Infrastructure.States;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;
using UnityEngine;

namespace Game.CodeBase.Infrastructure.States
{
    public class SearchServersState : IState
    {
        private readonly string MainMenu = "MainMenu";

        private readonly GameStateMachine _stateMachine;
        private readonly CustomNetworkManager _networkManager;
        private readonly ServersObserver _serversObserver;
        private readonly SceneLoader _sceneLoader;

        public SearchServersState(
            GameStateMachine gameStateMachine,
            SceneLoader sceneLoader,
            CustomNetworkManager networkManager,
            ServersObserver serversObserver)
        {
            _sceneLoader = sceneLoader;
            _stateMachine = gameStateMachine;
            _networkManager = networkManager;
            _serversObserver = serversObserver;
            _serversObserver.FoundServers += JoinOrHost;
        }

        public async void Enter()
        {
            await _sceneLoader.LoadAsync(MainMenu);

            _serversObserver.StartObservation();
        }

        private void JoinOrHost(Dictionary<long, ServerInfo> foundServers)
        {
            if (foundServers.Count == 0)
            {
                Debug.Log("Start host because 0");
                _stateMachine.Enter<HostLobbyState, ushort>(7777);
                return;
            }

            if (_networkManager.ConnectToAvailableServerAutomatically)
            {
                Debug.Log("Start client");
                var firstServerInfo = foundServers.First().Value;
                _stateMachine.Enter<JoinLobbyState, ServerInfo>(firstServerInfo);
            }
            else
            {
                Debug.Log("Start host");
                ushort newPort = (ushort) PortScanner.GetAvailablePort(4000);
                _stateMachine.Enter<HostLobbyState, ushort>(newPort);
            }
        }

        public void Exit()
        {
            _serversObserver.Stop();
        }
    }
}