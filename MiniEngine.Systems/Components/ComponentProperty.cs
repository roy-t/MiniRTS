using System;

namespace MiniEngine.Systems.Components
{
    public sealed class ComponentProperty
    {
        public ComponentProperty(string name, object value, Action<object> setter, Type type, MinMaxDescription minMax)
        {
            this.Name = name;
            this.Value = value;
            this.Setter = setter;
            this.Type = type;
            this.MinMax = minMax;
        }

        public string Name { get; }        
        public object Value { get; }
        public Action<object> Setter { get; }        
        public Type Type { get; }        
        public MinMaxDescription MinMax { get; }
    }
}
