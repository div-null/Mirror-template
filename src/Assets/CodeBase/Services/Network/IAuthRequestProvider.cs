namespace Game.CodeBase.Services.Network
{
    internal interface IAuthRequestProvider
    {
        ClientAuthenticator.AuthRequestMessage Request();
    }
}