using System;
using System.Collections.Generic;

namespace NeonBlack.Systems.StateMachine
{
    public class StateMachine<TBlackboard>
    {
        internal readonly TBlackboard Blackboard;
        private readonly Dictionary<Type, State<TBlackboard>> states = new();

        public StateMachine(TBlackboard blackboard)
        {
            Blackboard = blackboard;
        }

        public State<TBlackboard> CurrentState { get; private set; }

        public void SwitchState<T>(bool force = false, bool skipExit = false) where T : State<TBlackboard>, new()
        {
            var key = typeof(T);
            if (!states.TryGetValue(key, out var state))
            {
                state = new T();
                state.Register(this);
                states.Add(key, state);
            }

            if (state == CurrentState && !force)
            {
                return;
            }

            if (!skipExit)
            {
                CurrentState?.OnExit();
            }

            CurrentState = state;
            CurrentState.OnEnter();
        }

        public void Update(float deltaTime)
        {
            CurrentState?.OnUpdate(deltaTime);
        }
    }
}
