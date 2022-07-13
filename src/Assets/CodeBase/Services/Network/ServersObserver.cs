using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CodeBase.Services.Network
{
    public class ServersObserver
    {
        public event Action<Dictionary<long, ServerInfo>> FoundServers;
        public event Action<ServerInfo> FoundServer;

        private Dictionary<long, ServerInfo> _servers;
        private readonly INetworkDiscovery _networkDiscovery;
        private readonly ICoroutineRunner _runner;

        public ServersObserver(INetworkDiscovery discovery, ICoroutineRunner runner)
        {
            _runner = runner;
            _networkDiscovery = discovery;
            _networkDiscovery.OnServerFound += OnDiscoveredServer;
        }

        private void OnDiscoveredServer(ServerInfo server)
        {
            Debug.Log("Found server with url: " + server.Uri);
            _servers.Add(server.ServerId, server);
            FoundServer?.Invoke(server);
        }

        public void StartObservation()
        {
            _servers = new Dictionary<long, ServerInfo>();
            _runner.StartCoroutine(WaitForObservation());
        }

        public void AdvertiseServer() =>
            _networkDiscovery.AdvertiseServer();

        public void Reload()
        {
            Stop();
            StartObservation();
        }

        public void Stop() =>
            _networkDiscovery.StopDiscovery();

        IEnumerator WaitForObservation()
        {
            _networkDiscovery.StartDiscovery();
            yield return new WaitForSeconds(1);
            FoundServers?.Invoke(_servers);
        }
    }
}