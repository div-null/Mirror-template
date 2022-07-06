using Game.CodeBase.Infrastructure.Network;
using Game.CodeBase.States.CodeBase.Infrastructure.States;

namespace Game.CodeBase.Infrastructure
{
    public class Game
    {
        public GameStateMachine GameStateMachine;

        public Game(ICoroutineRunner coroutineRunner, AdvancedNetworkManager advancedNetworkManager)
        {
            GameStateMachine = new GameStateMachine(new SceneLoader(coroutineRunner), advancedNetworkManager, coroutineRunner);
            //Init state machine
            //auto host/connect будет в первом либо во втором стейте. игра грузит сцену с лобби,
            //дожидается пока прогрузится сцена, чтобы найти все комнаты и начать функционировать как лобби
        }
    }
}
