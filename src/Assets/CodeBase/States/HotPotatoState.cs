using CodeBase.Infrastructure;
using CodeBase.Infrastructure.CodeBase.Infrastructure.States;
using Mirror;

namespace CodeBase.States
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