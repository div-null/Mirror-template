using Game.CodeBase.Game;
using Game.CodeBase.Game.Lobby;
using Game.CodeBase.Infrastructure.ConnectionHandlers;
using Game.CodeBase.Infrastructure.States;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;
using Mirror;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.CodeBase.Infrastructure.Installers
{
    public class InitialScope : LifetimeScope
    {
        private class EntryPoint : IStartable
        {
            public void Start()
            {
                LifetimeScope lifetimeScope = Find<InitialScope>().CreateChild(new GameStateMachineInstaller());

                var gameStateMachine = lifetimeScope.Container.Resolve<IGameStateMachine>();
                var progressData = lifetimeScope.Container.Resolve<PlayerProgressData>();
                Debug.Log(progressData.Progress);
                gameStateMachine.Enter<BootstrapState>();
            }
        }

        protected override void Configure(IContainerBuilder builder)
        {
            LifetimeScope scope = Find<RootScope>();
            var networkManager = scope.Container.Resolve<CustomNetworkManager>();

            builder.Register<ServersObserver>(Lifetime.Singleton);
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<GameState>(Lifetime.Singleton);
            builder.Register<NetworkSpawner>(Lifetime.Singleton);

            RegisterNetworkHandlers(builder);

            builder.Register<LobbyFactory>(Lifetime.Singleton);
            builder.Register(_ => new PlayerFactory(networkManager.GetStartPosition(), networkManager.playerPrefab), Lifetime.Singleton);

            RegisterNetworkComponents(builder, networkManager);
            
            builder.RegisterEntryPoint<EntryPoint>();
        }

        private static void RegisterNetworkHandlers(IContainerBuilder builder)
        {
            builder.Register<AuthRequestProvider>(Lifetime.Singleton).As<IAuthRequestProvider>();
            builder.Register<DuplicateNameHandler>(Lifetime.Singleton).As<IAuthRequestHandler>();
            builder.Register<ServerNotifier>(Lifetime.Singleton).As<IServerNotifier>();
        }

        private static void RegisterNetworkComponents(IContainerBuilder builder, CustomNetworkManager networkManager) =>
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