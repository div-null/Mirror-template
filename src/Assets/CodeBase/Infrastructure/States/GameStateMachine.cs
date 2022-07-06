using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.CodeBase.Infrastructure.States
{
    namespace CodeBase.Infrastructure.States
    {
        public class GameStateMachine : IGameStateMachine
        {
            private Dictionary<Type, IExitableState> _states;
            private IExitableState _activeState;

            public void SetStates(IEnumerable<IExitableState> states)
            {
                _states = states.ToDictionary(state => state.GetType(), state => state);
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