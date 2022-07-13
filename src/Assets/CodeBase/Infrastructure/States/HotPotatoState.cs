using Game.CodeBase.Infrastructure.States.CodeBase.Infrastructure.States;
using Game.CodeBase.Services;
using Mirror;

namespace Game.CodeBase.Infrastructure.States
{
    public class HotPotatoState : IState
    {
        public HotPotatoState(GameStateMachine stateMachine, NetworkManager networkManager, ICoroutineRunner coroutineRunner)
        {
            
        }
        
        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void Enter()
        {
            throw new System.NotImplementedException();
        }
    }
}