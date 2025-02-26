namespace NeonBlack.Systems.StateMachine
{
    public abstract class StateMachineHelpers<TBlackboard>
    {
        private StateMachine<TBlackboard> stateMachine;

        protected TBlackboard Blackboard => stateMachine.Blackboard;
        protected TBlackboard Bb => stateMachine.Blackboard;

        internal void Register(StateMachine<TBlackboard> sm)
        {
            stateMachine = sm;
        }
    }
}
