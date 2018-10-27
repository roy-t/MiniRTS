using System.Collections.Generic;
using System.Linq;

namespace MiniEngine.Utilities
{
    public sealed class StateMachine<T, C>
        where T : struct
    {
        private readonly IReadOnlyList<StateChangeTrigger<T, C>> StateChangeTriggers;

        public StateMachine(T startState, params StateChangeTrigger<T, C>[] stateChangeTriggers)
        {
            this.State = startState;
            this.StateChangeTriggers = stateChangeTriggers.ToList().AsReadOnly();
        }

        public T State { get; private set; }

        public void Update(C context)
        {
            foreach (var trigger in this.StateChangeTriggers)
            {
                this.State = trigger.GetNextState(this.State, context);
            }
        }
    }
}
