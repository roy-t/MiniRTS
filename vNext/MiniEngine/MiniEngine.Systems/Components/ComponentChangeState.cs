namespace MiniEngine.Systems.Components
{
    public sealed class ComponentChangeState
    {
        public static ComponentChangeState NewComponent() => new ComponentChangeState();

        private ComponentChangeState()
        {
            this.CurrentState = LifetimeState.Created;
            this.NextState = LifetimeState.New;
        }

        internal LifetimeState CurrentState { get; private set; }
        internal LifetimeState NextState { get; set; }

        public void Change()
            => this.NextState = LifetimeState.Changed;

        public void Remove()
            => this.NextState = LifetimeState.Removed;

        public void Next()
        {
            this.CurrentState = this.NextState;
            this.NextState = LifetimeState.Unchanged;
        }

        public void Update(ChangeState state)
        {
            switch (state)
            {
                case ChangeState.Changed:
                    this.Change();
                    break;

                case ChangeState.Removed:
                    this.Remove();
                    break;
            }
        }
    }

    internal enum LifetimeState
    {
        Created,
        New,
        Changed,
        Unchanged,
        Removed
    }

    public enum ChangeState
    {
        Unchanged,
        Changed,
        Removed
    }
}
