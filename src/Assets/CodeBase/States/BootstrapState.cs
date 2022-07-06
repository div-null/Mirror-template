using Game.CodeBase.Infrastructure;
using Game.CodeBase.States.CodeBase.Infrastructure.States;

namespace Game.CodeBase.States
{
    public class BootstrapState : IState
    {
        private readonly string MainMenu = "MainMenu";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader)
        {
            _stateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            
            //Init inputs
            MainInputActions mainInputActions = new MainInputActions();
            mainInputActions.Enable();
            AllServices.MainInputActions = mainInputActions;
            //Init progress
            PlayerProgressData playerProgressData = new PlayerProgressData();
            AllServices.PlayerProgressData = playerProgressData;
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