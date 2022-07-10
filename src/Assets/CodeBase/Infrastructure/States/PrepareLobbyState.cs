using System.Collections.Generic;
using System.Linq;
using Game.CodeBase.Implementations;
using Game.CodeBase.Infrastructure.States.CodeBase.Infrastructure.States;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;
using kcp2k;
using Mirror;
using Mirror.Discovery;
using UnityEngine;

namespace Game.CodeBase.Infrastructure.States
{
    public class PrepareLobbyState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly AdvancedNetworkManager _advancedNetworkManager;
        private readonly ServersObserver _serversObserver;
        
        public PrepareLobbyState(GameStateMachine gameStateMachine, AdvancedNetworkManager advancedNetworkManager, ServersObserver serversObserver)
        {
            _stateMachine = gameStateMachine;
            _advancedNetworkManager = advancedNetworkManager;
            _serversObserver = serversObserver;
            _serversObserver.FoundServers += JoinOrHost;
        }

        public void Enter()
        {
            _serversObserver.StartObservation();
        }

        private void JoinOrHost(Dictionary<long, ServerInfo> foundServers)
        {
            _advancedNetworkManager.SetImplementator(new LobbyImplementator(_advancedNetworkManager));
            if (foundServers.Count == 0)
            {
                Debug.Log("Start host because 0");
                _advancedNetworkManager.StartHost();
                _serversObserver.AdvertiseServer();
            }
            else
            {
                if (_advancedNetworkManager.ConnectToAvailableServerAutomatically)
                {
                    Debug.Log("Start client");
                    //TODO: Change response information to find out if the server is available
                    var firstServerInfo = foundServers.First().Value;
                    _advancedNetworkManager.StartClient(firstServerInfo.Uri);
                }
                else
                {
                    Debug.Log("Start host");
                    ushort newPort = (ushort)PortScanner.GetAvailablePort(4000);
                    _advancedNetworkManager.gameObject.GetComponent<KcpTransport>().Port = newPort;
                    _advancedNetworkManager.StartHost();
                    _serversObserver.AdvertiseServer();
                }
            }
        }

        public void Exit()
        {
            _serversObserver.Stop();
        }
    }
}