using Game.CodeBase.Model;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;

namespace Game.CodeBase.Infrastructure.ConnectionHandlers
{
    public sealed class AuthRequestProvider : IAuthRequestProvider
    {
        private readonly PlayerProgressData _progressService;

        public AuthRequestProvider(PlayerProgressData progressService)
        {
            _progressService = progressService;
        }

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