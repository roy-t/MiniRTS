using System.Collections.Generic;

namespace MiniEngine.Systems.Components
{
    public interface IComponentContainer
    {
        IComponent this[int index] { get; }
        IComponent this[Entity entity] { get; }
        int Count { get; }
    }

    public class ComponentContainer<T> : IComponentContainer
        where T : IComponent
    {
        private readonly List<T> Components;
        private readonly Dictionary<Entity, T> LookUp;

        public ComponentContainer()
        {
            this.Components = new List<T>();
            this.LookUp = new Dictionary<Entity, T>();
        }

        public T this[int index] => this.Components[index];

        IComponent IComponentContainer.this[int index] => this[index];

        public T this[Entity entity] => this.LookUp[entity];

        IComponent IComponentContainer.this[Entity entity] => this[entity];

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
    }
}
