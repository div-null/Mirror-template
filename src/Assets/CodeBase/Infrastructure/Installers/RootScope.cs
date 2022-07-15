using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.CodeBase.Infrastructure.Installers
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] public InitialScope Bootstrapper;
        [SerializeField] private CustomNetworkManager _networkManager;

        private static bool _started;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerProgressData>(Lifetime.Singleton);
            builder.Register<MainInputActions>(Lifetime.Singleton);
            builder.Register(resolver => resolver.Instantiate(_networkManager), Lifetime.Singleton);
            if (!_started)
            {
                builder.RegisterEntryPoint<RootEntryPoint>();
                _started = true;
            }
        }
    }

    public class RootEntryPoint : IStartable
    {
        public void Start()
        {
            RootScope scope = (RootScope) LifetimeScope.Find<RootScope>();
            scope.CreateChildFromPrefab(scope.Bootstrapper);
        }
    }
}