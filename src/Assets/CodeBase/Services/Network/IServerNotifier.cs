using System.Net;

namespace Game.CodeBase.Services.Network
{
    public interface IServerNotifier
    {
        ServerInfo Notify(long serverId, DiscoveryRequest request, IPEndPoint endpoint);
    }
}