using System;
using System.Collections.Generic;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Containers
{
    public abstract class AComponentList<T> : IComponentContainer<T>
        where T : IComponent
    {
        private readonly List<T> Components;
        
        public AComponentList()
        {
            this.Components = new List<T>();
        }

        public void Add(T item) => this.Components.Add(item);
        public T this[int index] => this.Components[index];
        public bool Remove(T item) => this.Components.Remove(item);

        public int Count => this.Components.Count;

        public void RemoveAllOwnedBy(Entity entity)
        {
            for (var i = this.Components.Count - 1; i >= 0; i--)
            {
                if(this.Components[i].Entity == entity)
                {
                    this.Components.RemoveAt(i);
                }
            }
        }

        public Type GetComponentType() => typeof(T);
    }
}