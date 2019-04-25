using System;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Annotations
{
    public sealed class EditorAttribute : Attribute
    {
        public EditorAttribute(string name, string property)
            : this(name, property, float.MinValue, float.MaxValue) { }


        public EditorAttribute(string name, string getter, string setter)
            : this(name, getter, setter, float.MinValue, float.MaxValue) { }

        public EditorAttribute(string name, string property, float min, float max)
            : this(name, property, property, min, max) { }

        public EditorAttribute(string name, string getter, string setter, float min, float max)
        {
            this.Name = name;
            this.Getter = getter;
            this.Setter = setter;
            this.MinMax = new MinMaxDescription(min, max);
        }

        public string Name { get; }
        public string Getter { get; }
        public string Setter { get; }
        public MinMaxDescription MinMax { get; }
    }
}
