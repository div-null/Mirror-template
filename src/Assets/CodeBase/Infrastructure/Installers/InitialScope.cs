using Game.CodeBase.Infrastructure.States;
using Game.CodeBase.Services.Network;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.CodeBase.Infrastructure.Installers
{
    public class InitialScope : LifetimeScope
    {
        [SerializeField] private AdvancedNetworkManager _advancedNetworkManagerPrefab;

        class EntryPoint : IStartable
        {
            private readonly IGameStateMachine _gameStateMachine;

            public EntryPoint(IGameStateMachine gameStateMachine)
            {
                _gameStateMachine = gameStateMachine;
            }

            public void Start()
            {
                _gameStateMachine.Enter<BootstrapState>();
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>();
            builder.Register<MainInputActions>(Lifetime.Singleton);
            builder.Register<PlayerProgressData>(Lifetime.Singleton);

            LifetimeScope stateMachineScope = CreateChild(new GameStateMachineInstaller());
            var gameStateMachine = stateMachineScope.Container.Resolve<IGameStateMachine>();
            builder.Register(_ => gameStateMachine, Lifetime.Singleton);
        }
    }
}