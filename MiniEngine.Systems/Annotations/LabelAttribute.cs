using System;

namespace MiniEngine.Systems.Annotations
{
    public sealed class LabelAttribute : Attribute
    {
        public LabelAttribute(string label)
        {
            this.Label = label;
        }

        public string Label { get; }
    }
}
