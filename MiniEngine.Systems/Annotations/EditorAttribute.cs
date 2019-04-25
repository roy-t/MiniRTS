using System;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Annotations
{
    public sealed class EditorAttribute : Attribute
    {
        public EditorAttribute(string name)
            : this(name, null, float.MinValue, float.MaxValue) { }


        public EditorAttribute(string name, string setter)
            : this(name, setter, float.MinValue, float.MaxValue) { }       

        public EditorAttribute(string name, string setter, float min, float max)
        {
            this.Name = name;
            this.Setter = setter;
            this.MinMax = new MinMaxDescription(min, max);
        }

        public string Name { get; }
        public string Setter { get; }
        public MinMaxDescription MinMax { get; }
    }
}
