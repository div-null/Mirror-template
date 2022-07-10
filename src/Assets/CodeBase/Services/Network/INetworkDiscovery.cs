using System;

namespace Game.CodeBase.Services.Network
{
    public interface INetworkDiscovery
    {
        event Action<ServerInfo> OnServerFound;
        void Initialize(string serverName, int playersCount = 1, int maxPlayers = 4);
        void UpdatePlayersCount(int playersCount, int maxPlayers = 4);

        /// <summary>
        /// Advertise this server in the local network
        /// </summary>
        void AdvertiseServer();

        /// <summary>
        /// Start Active Discovery
        /// </summary>
        void StartDiscovery();

        /// <summary>
        /// Stop Active Discovery
        /// </summary>
        void StopDiscovery();
    }
}