using System;

namespace MiniEngine.Gui.Tools
{
    public abstract class ATool<T> : ITool
    {
        public Type TargetType => typeof(T);

        public abstract string Name { get; }

        public virtual int Priority => 0;

        public bool HeaderValue(ref object value, ToolState tool)
        {
            var specific = (T)value;
            var changed = this.HeaderValue(ref specific, tool);
            value = specific!;

            return changed;
        }

        public abstract bool HeaderValue(ref T value, ToolState tool);

        public virtual bool Details(ref T value, ToolState tool) => false;

        public bool Details(ref object value, ToolState tool, Property propertyPath)
        {
            var specific = (T)value;
            var changed = this.Details(ref specific, tool);
            value = specific!;

            return changed;
        }

        public virtual ToolState Configure(ToolState tool) => tool;
    }
}
