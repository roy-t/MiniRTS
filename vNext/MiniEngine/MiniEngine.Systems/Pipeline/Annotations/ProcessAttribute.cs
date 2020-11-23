using System;

namespace MiniEngine.Systems.Pipeline.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProcessAttribute : Attribute
    {
        public ProcessAttribute(ComponentState filter)
        {
            this.Filter = filter;
        }

        public ComponentState Filter { get; }
    }

    public enum ComponentState
    {
        New,
        Changed,
        Unchanged,
    }
}
