namespace MiniEngine.Systems.Components
{
    public sealed class ComponentChangeState
    {
        public static ComponentChangeState NewComponent() => new ComponentChangeState();

        private ComponentChangeState()
        {
            this.CurrentState = ChangeState.Initialized;
            this.NextState = ChangeState.New;
        }

        public ChangeState CurrentState { get; private set; }
        public ChangeState NextState { get; set; }

        public void MarkChanged()
            => this.NextState = ChangeState.Changed;

        public void Next()
        {
            this.CurrentState = this.NextState;
            this.NextState = ChangeState.Unchanged;
        }
    }

    public enum ChangeState
    {
        Initialized,
        New,
        Changed,
        Unchanged,
    }
}
