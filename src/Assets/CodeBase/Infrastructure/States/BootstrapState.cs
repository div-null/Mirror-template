using Game.CodeBase.Infrastructure.States.CodeBase.Infrastructure.States;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;

namespace Game.CodeBase.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly string MainMenu = "MainMenu";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly MainInputActions _inputs;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, MainInputActions inputs)
        {
            _stateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _inputs = inputs;
        }

        public void Enter()
        {
            _inputs.Enable();
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