using System;

namespace MiniEngine.Gui.Tools
{
    public abstract class ATool<T> : ITool
    {
        public Type TargetType => typeof(T);

        public abstract string Name { get; }

        public abstract T Select(T value, Property property, ToolState tool);

        public virtual ToolState Configure(ToolState tool) => tool;
    }
}
