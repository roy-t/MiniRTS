using System;

namespace MiniEngine.Systems.Annotations
{
    public sealed class IconAttribute : Attribute
    {
        public IconAttribute(IconType type)
        {
            this.Type = type;
        }

        public IconType Type { get; }
    }
}
