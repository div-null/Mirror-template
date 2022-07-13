using System;
using System.Linq;
using System.Net;
using Mirror;
using UnityEngine;

using Game.CodeBase.Game;
using Game.CodeBase.Services.Network;

namespace Game.CodeBase.Infrastructure
{
    class ServerNotifier : IServerNotifier
    {
        private readonly Transport _transport;
        private readonly GameState _gameState;

        public ServerNotifier(GameState gameState)
        {
            _gameState = gameState;
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
                    ServerName = _gameState.ServerName,
                    MaxPlayers = _gameState.MaxPlayers,
                    PlayersCount = _gameState.Players.Count(player => player != null)
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