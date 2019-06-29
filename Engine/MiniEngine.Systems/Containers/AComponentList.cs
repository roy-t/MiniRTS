using System;
using System.Collections.Generic;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Containers
{
    public sealed class ComponentList<T> : IComponentContainer<T>
        where T : IComponent
    {
        private readonly List<T> Components;
        
        public ComponentList()
        {
            this.Components = new List<T>();
        }

        public void Add(T item) => this.Components.Add(item);        
        public bool Remove(T item) => this.Components.Remove(item);
        public bool Remove(IComponent component) => this.Components.Remove((T)component);
        public int Count => this.Components.Count;
        public void Clear() => this.Components.Clear();

        public T this[int index] => this.Components[index];
        IComponent IComponentContainer.this[int index] => this.Components[index];

        public void RemoveAllOwnedBy(Entity entity)
        {
            for (var i = this.Components.Count - 1; i >= 0; i--)
            {
                if (this.Components[i].Entity == entity)
                {
                    this.Components.RemoveAt(i);
                }
            }
        }

        public Type GetComponentType() => typeof(T);
        
    }
}