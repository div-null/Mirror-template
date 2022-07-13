using System.Linq;
using JetBrains.Annotations;
using Mirror;
using Unity.VisualScripting;
using Game.CodeBase.Game;
using Game.CodeBase.Model;
using Game.CodeBase.Services.Network;

namespace Game.CodeBase.Infrastructure.ConnectionHandlers
{
    public sealed class DuplicateNameHandler : IAuthRequestHandler
    {
        private readonly GameState _gameState;

        public DuplicateNameHandler(GameState gameState)
        {
            _gameState = gameState;
        }

        public bool Accept(NetworkConnectionToClient conn, ClientAuthenticator.AuthRequestMessage msg, out ClientAuthenticator.AuthResponseMessage authResponseMessage)
        {
            int nameDuplicates = _gameState.Players
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