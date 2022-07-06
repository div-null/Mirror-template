using Game.CodeBase.Infrastructure.States.CodeBase.Infrastructure.States;
using Game.CodeBase.Services;

namespace Game.CodeBase.Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly string MainMenu = "MainMenu";
        private readonly GameStateMachine _stateMachine;
        private readonly SceneLoader _sceneLoader;
        private readonly MainInputActions _inputs;
        private readonly PlayerProgressData _playerProgressData;

        public BootstrapState(GameStateMachine gameStateMachine, SceneLoader sceneLoader, MainInputActions inputs, PlayerProgressData playerProgressData)
        {
            _stateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _inputs = inputs;
            _playerProgressData = playerProgressData;
        }

        public void Enter()
        {
            _inputs.Enable();
            _playerProgressData.Load();
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