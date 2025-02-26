namespace NeonBlack.Systems.StateMachine
{
    public abstract class State<TBlackboard>
    {
        private StateMachine<TBlackboard> stateMachine;

        internal State()
        {
        }

        protected TBlackboard Blackboard => stateMachine.Blackboard;
        protected TBlackboard Bb => stateMachine.Blackboard;

        protected State<TBlackboard> CurrentState => stateMachine.CurrentState;

        protected void SwitchState<T>(bool forced = false, bool skipExit = false) where T : State<TBlackboard>, new()
        {
            stateMachine.SwitchState<T>(forced, skipExit);
        }

        internal virtual void Register(StateMachine<TBlackboard> sm)
        {
            stateMachine = sm;
        }

        internal abstract void OnExit();
        internal abstract void OnEnter();
        internal abstract void OnUpdate(float deltaTime);
    }

    public abstract class State<TBlackboard, THelpers> : State<TBlackboard>
        where THelpers : StateMachineHelpers<TBlackboard>, new()
    {
        protected THelpers Helpers { get; } = new();
        protected THelpers H => Helpers;

        internal override void Register(StateMachine<TBlackboard> sm)
        {
            base.Register(sm);
            Helpers.Register(sm);
        }
    }
}
