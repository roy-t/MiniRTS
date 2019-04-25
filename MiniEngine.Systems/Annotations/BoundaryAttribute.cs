using System;

namespace MiniEngine.Systems.Annotations
{
    public sealed class BoundaryAttribute : Attribute
    {
        public BoundaryAttribute(BoundaryType type)
        {
            this.Type = type;
        }

        public BoundaryType Type { get; }
    }
}
