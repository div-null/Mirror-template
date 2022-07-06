using Game.CodeBase.Infrastructure;
using Game.CodeBase.States.CodeBase.Infrastructure.States;
using Mirror;

namespace Game.CodeBase.States
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