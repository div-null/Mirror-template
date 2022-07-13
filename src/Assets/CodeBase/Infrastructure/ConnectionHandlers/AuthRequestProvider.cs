using CodeBase.Model;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;

namespace Game.CodeBase.Infrastructure.ConnectionHandlers
{
    class AuthRequestProvider : IAuthRequestProvider
    {
        private PlayerProgressData _progressService;

        public ClientAuthenticator.AuthRequestMessage Request()
        {
            PlayerProgress progress = _progressService.Progress;

            return new ClientAuthenticator.AuthRequestMessage
            {
                Username = progress.Username,
                Color = progress.ColorData.Color
            };
        }
    }
}