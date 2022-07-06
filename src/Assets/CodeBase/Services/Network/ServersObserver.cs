using System;
using System.Collections;
using System.Collections.Generic;
using Game.CodeBase.Infrastructure;
using Mirror.Discovery;
using UnityEngine;

namespace Game.CodeBase.Services.Network
{
    public class ServersObserver
    {
        public event Action<Dictionary<long, ServerResponse>> FoundServers;
        public event Action<ServerResponse> FoundServer;
        
        private Dictionary<long, ServerResponse> _servers;
        private readonly NetworkDiscovery _networkDiscovery;
        private readonly ICoroutineRunner _runner;

        public ServersObserver(NetworkDiscovery discovery, ICoroutineRunner runner)
        {
            _runner = runner;
            _networkDiscovery = discovery;
            _networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        }

        public void StartObservation()
        {
            _servers = new Dictionary<long, ServerResponse>();
            _runner.StartCoroutine(WaitForObservation());
        }

        public void AdvertiseServer() => 
            _networkDiscovery.AdvertiseServer();

        public void Reload()
        {
            _networkDiscovery.StopDiscovery();
            StartObservation();
        }

        IEnumerator WaitForObservation()
        {
            _networkDiscovery.StartDiscovery();
            yield return new WaitForSeconds(1);
            FoundServers?.Invoke(_servers);
        }

        void OnDiscoveredServer(ServerResponse serverResponse)
        {
            Debug.Log("Found server with port: " + serverResponse.uri.Port);
            _servers.Add(serverResponse.serverId, serverResponse);
            FoundServer?.Invoke(serverResponse);
        }
    }
}
