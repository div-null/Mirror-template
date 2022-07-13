using System.Collections.Generic;
using System.Linq;
using Game.CodeBase.Game;
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
        private readonly ServersObserver _serversObserver;
        private readonly SceneLoader _sceneLoader;
        private readonly GameState _gameState;

        public SearchServersState(
            GameStateMachine gameStateMachine,
            SceneLoader sceneLoader,
            GameState gameState,
            ServersObserver serversObserver)
        {
            _sceneLoader = sceneLoader;
            _stateMachine = gameStateMachine;
            _gameState = gameState;
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

            if (_gameState.ConnectToAvailableServerAutomatically)
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