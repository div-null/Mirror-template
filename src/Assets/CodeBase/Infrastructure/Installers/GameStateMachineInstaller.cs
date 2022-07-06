using System.Collections.Generic;
using Game.CodeBase.Infrastructure.States;
using Game.CodeBase.Infrastructure.States.CodeBase.Infrastructure.States;
using VContainer;
using VContainer.Unity;

namespace Game.CodeBase.Infrastructure.Installers
{
    public class GameStateMachineInstaller : IInstaller
    {
        private IGameStateMachine _gameStateMachine;
        public void Install(IContainerBuilder builder)
        {
            builder.Register<BootstrapState>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PrepareLobbyState>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<HotPotatoState>(Lifetime.Scoped).AsImplementedInterfaces();
            builder.Register<GameStateMachine>(Lifetime.Singleton).As<IGameStateMachine>().AsSelf();
            builder.RegisterBuildCallback(container =>
            {
                var stateMachine = container.Resolve<GameStateMachine>();
                var states = container.Resolve<IEnumerable<IExitableState>>();
                stateMachine.SetStates(states);
            });
        }
    }
}