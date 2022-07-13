using Mirror;

namespace Game.CodeBase.Services.Network
{
    public interface IAuthRequestHandler
    {
        bool Accept(NetworkConnectionToClient conn, ClientAuthenticator.AuthRequestMessage msg, out ClientAuthenticator.AuthResponseMessage authResponseMessage);
    }
}