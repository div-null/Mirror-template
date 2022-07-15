using System;
using Mirror;

namespace Game.CodeBase.Shared
{
    public static class NetworkingExtensions
    {
        [Obsolete]
        public static bool HasAuthority(this NetworkBehaviour entity, NetworkBehaviour other) =>
            entity.connectionToServer?.connectionId == other.connectionToServer?.connectionId ||
            entity.connectionToClient?.connectionId == other.connectionToClient?.connectionId;
    }
}