using Game.CodeBase.Infrastructure.States;
using VContainer;
using VContainer.Unity;

namespace Game.CodeBase.Infrastructure.Installers
{
    public class GameStateMachineInstaller : IInstaller
    {
        private IGameStateMachine _gameStateMachine;
        public void Install(IContainerBuilder builder)
        {
            builder.Register<BootstrapState>(Lifetime.Scoped);
            builder.Register<PrepareLobbyState>(Lifetime.Scoped);
            builder.Register<HotPotatoState>(Lifetime.Scoped);
        }
    }
}