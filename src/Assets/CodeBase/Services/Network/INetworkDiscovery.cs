using System;

namespace Game.CodeBase.Services.Network
{
    public interface INetworkDiscovery
    {
        event Action<ServerInfo> OnServerFound;

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