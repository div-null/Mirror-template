using Game.CodeBase.Game;
using Game.CodeBase.Game.Lobby;
using Game.CodeBase.Infrastructure.ConnectionHandlers;
using Game.CodeBase.Infrastructure.States;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;

using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Game.CodeBase.Infrastructure.Installers
{
    public class InitialScope : LifetimeScope
    {
        [FormerlySerializedAs("_advancedNetworkManagerPrefab"), SerializeField]
        private CustomNetworkManager customNetworkManagerPrefab;

        private class EntryPoint : IStartable
        {
            private readonly InitialScope _scope;

            public EntryPoint(InitialScope scope)
            {
                _scope = scope;
            }

            public void Start()
            {
                LifetimeScope lifetimeScope = _scope.CreateChild(new GameStateMachineInstaller());

                var gameStateMachine = lifetimeScope.Container.Resolve<IGameStateMachine>();
                var progressData = lifetimeScope.Container.Resolve<PlayerProgressData>();
                Debug.Log(progressData.Progress);
                gameStateMachine.Enter<BootstrapState>();
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ServersObserver>(Lifetime.Singleton);
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<PlayerProgressData>(Lifetime.Singleton);
            builder.Register<MainInputActions>(Lifetime.Singleton);
            builder.Register<GameState>(Lifetime.Singleton);
            builder.RegisterEntryPoint<EntryPoint>();

            builder.Register<AuthRequestProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DuplicateNameHandler>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<ServerNotifier>(Lifetime.Singleton).AsImplementedInterfaces();

            CustomNetworkManager networkManager = Instantiate(customNetworkManagerPrefab);

            builder.Register<LobbyFactory>(Lifetime.Singleton);
            builder.Register(_ => new PlayerFactory(networkManager.GetStartPosition(), networkManager.playerPrefab), Lifetime.Singleton);

            builder.UseComponents(components =>
            {
                components.AddInstance(networkManager)
                    .As<ICoroutineRunner>()
                    .As<NetworkManager>()
                    .AsSelf();

                components.AddInstance(networkManager.GetComponent<CustomNetworkDiscovery>())
                    .As<INetworkDiscovery>()
                    .AsSelf();

                components.AddInstance(networkManager.GetComponent<ClientAuthenticator>());
            });
        }
    }
}