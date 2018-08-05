using System;

namespace MiniEngine.Utilities
{
    public sealed class StateChangeTrigger<T, C> where T : struct 
    {
        private readonly T Before;
        private readonly T After;
        private readonly Func<C, bool> ConditionFunc;

        public StateChangeTrigger(T before, T after, Func<C, bool> conditionFunc)
        {
            this.Before = before;
            this.After = after;
            this.ConditionFunc = conditionFunc;
        }

        public T GetNextState(T currentState, C context)
        {
            if (currentState.Equals(this.Before) && this.ConditionFunc(context))
            {
                return this.After;
            }

            return currentState;
        }        
    }
}
