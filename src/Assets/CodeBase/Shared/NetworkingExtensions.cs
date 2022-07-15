using Mirror;

namespace Game.CodeBase.Shared
{
    public static class NetworkingExtensions
    {
        public static bool HasAuthority(this NetworkBehaviour entity, NetworkBehaviour other) =>
            entity.connectionToServer?.connectionId == other.connectionToServer?.connectionId ||
            entity.connectionToClient?.connectionId == other.connectionToClient?.connectionId;
    }
}