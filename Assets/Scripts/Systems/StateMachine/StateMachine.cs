using System;
using System.Collections.Generic;
using R3;

namespace NeonBlack.Systems.StateMachine
{
    public class StateMachine<TBlackboard>
    {
        internal readonly TBlackboard Blackboard;
        private readonly ReactiveProperty<State<TBlackboard>> currentState = new();
        private readonly Dictionary<Type, State<TBlackboard>> states = new();

        public StateMachine(TBlackboard blackboard)
        {
            Blackboard = blackboard;
        }

        public ReadOnlyReactiveProperty<State<TBlackboard>> CurrentStateReactive => currentState;
        public State<TBlackboard> CurrentState => currentState.CurrentValue;

        public void SwitchState<T>(bool force = false, bool skipExit = false) where T : State<TBlackboard>, new()
        {
            var key = typeof(T);
            if (!states.TryGetValue(key, out var state))
            {
                state = new T();
                state.Register(this);
                states.Add(key, state);
            }

            if (state == currentState.CurrentValue && !force)
            {
                return;
            }

            if (!skipExit)
            {
                CurrentState?.OnExit();
            }

            currentState.Value = state;
            CurrentState?.OnEnter();
        }

        public void Update(float deltaTime)
        {
            CurrentState?.OnUpdate(deltaTime);
        }
    }
}
