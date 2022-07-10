using Game.CodeBase.Infrastructure.States.CodeBase.Infrastructure.States;

namespace Game.CodeBase.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly MainInputActions _inputs;

        public BootstrapState(GameStateMachine gameStateMachine, MainInputActions inputs)
        {
            _stateMachine = gameStateMachine;
            _inputs = inputs;
        }

        public void Enter()
        {
            _inputs.Enable();
            _stateMachine.Enter<SearchServersState>();
        }

        public void Exit()
        {
        }
    }
}