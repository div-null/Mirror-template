using Game.CodeBase.Infrastructure.Network;
using Game.CodeBase.States;
using UnityEngine;

namespace Game.CodeBase.Infrastructure
{
    public class GameBootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private AdvancedNetworkManager _advancedNetworkManagerPrefab;
        public Game Game;
    
        private void Awake()
        {
            Game = new Game(this, Instantiate(_advancedNetworkManagerPrefab));
            Game.GameStateMachine.Enter<BootstrapState>();
        }
    }
}
