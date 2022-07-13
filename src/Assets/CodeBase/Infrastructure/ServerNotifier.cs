using System;
using System.Linq;
using System.Net;
using Game.CodeBase.Services.Network;
using Mirror;
using UnityEngine;

namespace Game.CodeBase.Infrastructure
{
    class ServerNotifier : IServerNotifier
    {
        private readonly Transport _transport;
        private readonly CustomNetworkManager _networkManager;

        public ServerNotifier(CustomNetworkManager networkManager)
        {
            _networkManager = networkManager;
            _transport = Transport.activeTransport;
        }

        public ServerInfo Notify(long serverId, DiscoveryRequest request, IPEndPoint endpoint)
        {
            try
            {
                // this is an example reply message,  return your own
                // to include whatever is relevant for your game
                return new ServerInfo
                {
                    ServerId = serverId,
                    Uri = _transport.ServerUri(),
                    ServerName = _networkManager.ServerName,
                    MaxPlayers = _networkManager.maxConnections,
                    PlayersCount = _networkManager.Players.Count(player => player != null)
                };
            }
            catch (NotImplementedException)
            {
                Debug.LogError($"Transport {_transport} does not support network discovery");
                throw;
            }
        }
    }
}