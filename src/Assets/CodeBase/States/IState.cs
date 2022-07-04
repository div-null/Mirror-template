namespace CodeBase.States
{
    public interface IState : IExitableState
    {
        void Enter();
    }

    public interface IPayloadedState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }
    
    public interface IExitableState
    {
        void Exit();
    }
}