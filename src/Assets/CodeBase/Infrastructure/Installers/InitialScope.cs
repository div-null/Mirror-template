using Game.CodeBase.Infrastructure.States;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;
using Mirror;
using Mirror.Discovery;
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
            private InitialScope _scope;

            public EntryPoint(InitialScope scope)
            {
                _scope = scope;
            }

            public void Start()
            {
                LifetimeScope lifetimeScope = _scope.CreateChild(new GameStateMachineInstaller());

                var gameStateMachine = lifetimeScope.Container.Resolve<IGameStateMachine>();
                var progressData = lifetimeScope.Container.Resolve<PlayerProgressData>();

                progressData.Load();
                gameStateMachine.Enter<BootstrapState>();
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ServersObserver>(Lifetime.Singleton);
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<PlayerProgressData>(Lifetime.Singleton);
            builder.Register<MainInputActions>(Lifetime.Singleton);
            builder.RegisterEntryPoint<EntryPoint>();
            
            AdvancedNetworkManager networkManager = Instantiate(_advancedNetworkManagerPrefab);
            
            builder.UseComponents(components =>
            {
                components.AddInstance(networkManager)
                    .As<ICoroutineRunner>()
                    .As<NetworkManager>()
                    .AsSelf();

                components.AddInstance(networkManager.GetComponent<NetworkDiscovery>());
            });
            
        }
    }
}