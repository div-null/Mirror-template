using System;
using System.Collections.Generic;
using Game.CodeBase.Infrastructure;
using Game.CodeBase.Services.Network;

namespace Game.CodeBase.States
{
    namespace CodeBase.Infrastructure.States
    {
        public class GameStateMachine : IGameStateMachine
        {
            //private AdvancedNetworkManager _advancedNetworkManager;
            //private AllServices _serviceContainer;
            //private GameJudger _gameJudger;
            private readonly Dictionary<Type, IExitableState> _states;
            private IExitableState _activeState;
            //private ICoroutineRunner _coroutineRunner;

            public GameStateMachine(SceneLoader sceneLoader, AdvancedNetworkManager advancedNetworkManager, ICoroutineRunner coroutineRunner)
            {
                
                //LobbyState
                //все готовы и перемещаются в стейт настройки миниигр -> GameOptions
                //настройки сохраняются и отдаются GameJudge, чтобы он знал, когда роисзойдет победа и на какие миниигры нужно переключать игроков
                //далее есть стейты всех миниигр, которые Enter'ятся и Exit'ются, когда заканчивается миниигра
                //началом и концом миниигры управляет Implementator. Он в итоге и вызывает Enter и Exit
                
                //При игре происходит следующее: GameJudge дает миниигры, происходит переход к ней с помощью State с Impementator,
                //она заканчиавется и происходит переход к сцене подведения итогов с очками (это всегда так)
                //далее после подведения итогов GameJudge снова подкидывает определенную миниигру
                
                //потом на последней миниигре должен сработать не промежуточное подведение итогов, а общее
                //далее происходит переход в лобби, где всем вручаются их очки
                
                _states = new Dictionary<Type, IExitableState>()
                {
                    [typeof(BootstrapState)] = new BootstrapState(this, sceneLoader),
                    [typeof(PrepareLobbyState)] = new PrepareLobbyState(this, advancedNetworkManager),
                    [typeof(HotPotatoState)] = new HotPotatoState(this, advancedNetworkManager, coroutineRunner)
                };
            }

            public void Enter<TState>() where TState : class, IState
            {
                var state = ChangeState<TState>();
                state.Enter();
            }

            public void Enter<TState, TPayLoad>(TPayLoad payLoad) where TState : class, IPayloadedState<TPayLoad>
            {
                TState state = ChangeState<TState>();
                state.Enter(payLoad);
            }

            private TState ChangeState<TState>() where TState : class, IExitableState
            {
                _activeState?.Exit();
                TState state = GetState<TState>();
                _activeState = state;
                return state;
            }

            private TState GetState<TState>() where TState : class, IExitableState => _states[typeof(TState)] as TState;
        }
    }
}