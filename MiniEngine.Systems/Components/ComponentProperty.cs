using System;

namespace MiniEngine.Systems.Components
{
    public sealed class ComponentProperty
    {
        public ComponentProperty(string name, object value, Action<object> setter, Type type, object min, object max)
        {
            this.Name = name;
            this.Value = value;
            this.Setter = setter;
            this.Type = type;
            this.Min = min;
            this.Max = max;
        }

        public string Name { get; }        
        public object Value { get; }
        public Action<object> Setter { get; }        
        public Type Type { get; }
        
        public object Min { get; }
        public object Max { get; }
    }
}
