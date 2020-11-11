using System;
using System.Collections.Generic;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Components
{
    public interface IComponentContainer
    {
        AComponent this[int index] { get; }

        AComponent this[Entity entity] { get; }

        int Count { get; }

        Type ComponentType { get; }

        void AddUnsafe(AComponent component);

        bool Contains(Entity entity);
    }

    [ComponentContainer]
    public class ComponentContainer<T> : IComponentContainer
        where T : AComponent
    {
        private readonly List<T> Components;
        private readonly Dictionary<Entity, T> LookUp;

        public ComponentContainer()
        {
            this.Components = new List<T>();
            this.LookUp = new Dictionary<Entity, T>();
        }

        public Type ComponentType => typeof(T);

        public T this[int index] => this.Components[index];

        AComponent IComponentContainer.this[int index] => this[index];

        public T this[Entity entity] => this.LookUp[entity];

        AComponent IComponentContainer.this[Entity entity] => this[entity];

        public void AddUnsafe(AComponent component) => this.Add((T)component);

        public void Add(T item)
        {
            this.Components.Add(item);
            this.LookUp.Add(item.Entity, item);
        }

        public void Remove(Entity entity)
        {
            if (this.LookUp.TryGetValue(entity, out var component))
            {
                this.Components.Remove(component);
                this.LookUp.Remove(entity);
            }
        }

        public T Get(Entity entity) => this.LookUp[entity];

        public int Count => this.Components.Count;

        public bool Contains(Entity entity) => this.LookUp.ContainsKey(entity);
    }
}
