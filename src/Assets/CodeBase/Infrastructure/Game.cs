using Game.CodeBase.Services.Network;
using Game.CodeBase.States.CodeBase.Infrastructure.States;

namespace Game.CodeBase.Infrastructure
{
    public class Game
    {
        public readonly GameStateMachine GameStateMachine;

        public Game(ICoroutineRunner coroutineRunner, AdvancedNetworkManager advancedNetworkManager)
        {
            GameStateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), advancedNetworkManager, coroutineRunner);
        }
    }
}
