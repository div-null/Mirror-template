using Game.CodeBase.Infrastructure.States.CodeBase.Infrastructure.States;

namespace Game.CodeBase.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly string MainMenu = "MainMenu";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, MainInputActions inputs)
        {
            _stateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            inputs.Enable();
        }

        public void Enter()
        {
            _sceneLoader.Load(MainMenu, onLoaded: EnterLobbyPreparer);
        }

        private void EnterLobbyPreparer()
        {
            _stateMachine.Enter<PrepareLobbyState>();
        }

        public void Exit()
        {
            
        }
    }
}