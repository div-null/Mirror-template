namespace Game.CodeBase.Services.Network
{
    public interface IAuthRequestProvider
    {
        ClientAuthenticator.AuthRequestMessage Request();
    }
}