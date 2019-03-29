using System;
using System.Collections.Generic;

namespace MiniEngine.Systems.Components
{
    public sealed class ComponentDescription
    {
        public ComponentDescription(string name)
        {
            this.Name = name;
            this.Properties = new List<ComponentProperty>();
        }

        public string Name { get; }
        public List<ComponentProperty> Properties { get; }

        public void AddProperty<T>(string name, T value, Action<T> setter, MinMaxDescription minMax) 
            => this.Properties.Add(new ComponentProperty(name, value, x => setter((T)x), typeof(T), minMax));

        public void AddLabel<T>(string name, T value)
            => this.Properties.Add(new ComponentProperty(name, value, null, typeof(T), MinMaxDescription.None));
    }
}
