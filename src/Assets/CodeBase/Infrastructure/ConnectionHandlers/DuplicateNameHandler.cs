using System.Linq;
using CodeBase.Model;
using Game.CodeBase.Services.Network;
using JetBrains.Annotations;
using Mirror;
using Unity.VisualScripting;

namespace Game.CodeBase.Infrastructure.ConnectionHandlers
{
    public sealed class DuplicateNameHandler : IAuthRequestHandler
    {
        private readonly CustomNetworkManager _networkManager;

        public DuplicateNameHandler(CustomNetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public bool Accept(NetworkConnectionToClient conn, ClientAuthenticator.AuthRequestMessage msg, out ClientAuthenticator.AuthResponseMessage authResponseMessage)
        {
            int nameDuplicates = _networkManager.Players
                .NotNull()
                .Count(player => player.Username.StartsWith(msg.Username));

            var updatedProgress = new PlayerProgress(DuplicateName(msg.Username, nameDuplicates), msg.Color);
            conn.authenticationData = updatedProgress;

            authResponseMessage = Respond(updatedProgress.Username);
            return true;
        }

        private static string DuplicateName(string name, int nameDuplicates) =>
            nameDuplicates > 0 ? name + $" ({nameDuplicates})" : name;

        private static ClientAuthenticator.AuthResponseMessage Respond([CanBeNull] string updatedName = null)
        {
            return updatedName != null
                ? ClientAuthenticator.AuthResponseMessage.FixedName(updatedName)
                : ClientAuthenticator.AuthResponseMessage.Success();
        }
    }
}