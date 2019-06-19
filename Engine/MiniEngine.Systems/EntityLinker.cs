using MiniEngine.Systems.Components;
using System;
using System.Collections.Generic;

namespace MiniEngine.Systems
{
    public sealed class EntityLinker
    {
        private readonly ComponentLookup LookUp;

        public EntityLinker()
        {
            this.LookUp = new ComponentLookup();
        }

        public void AddComponent(Entity entity, IComponent component) => this.LookUp.AddComponent(entity, component);

        public void RemoveComponent(Entity entity, IComponent component) => this.LookUp.RemoveComponent(entity, component);

        public void RemoveComponents<T>(Entity entity)
            where T : IComponent
            => this.LookUp.RemoveComponent(entity, typeof(T));

        public void RemoveComponents<T>()
            where T : IComponent
            => this.LookUp.RemoveAllComponents(typeof(T));

        public void GetComponents(Entity entity, IList<IComponent> output)
        {
            var components = this.LookUp.Search(entity);
            for (var i = 0; i < components.Count; i++)
            {
                output.Add(components[i].Component);
            }
        }

        public void GetComponents<T>(Entity entity, IList<T> output)
            where T : IComponent
        {
            var components = this.LookUp.Search(entity);
            for (var i = 0; i < components.Count; i++)
            {
                if (typeof(T).IsAssignableFrom(components[i].Component.GetType()))
                {
                    output.Add((T)components[i].Component);
                }
            }
        }

        public T GetComponent<T>(Entity entity)
            where T : IComponent
        {            
            var components = this.LookUp.Search(entity);
            for (var i = 0; i < components.Count; i++)
            {
                if (typeof(T).IsAssignableFrom(components[i].Component.GetType()))
                {
                    return (T)components[i].Component;
                }
            }

            throw new Exception($"Failed to find component of type {typeof(T)} for entity {entity}");
        }

        public void GetComponents<T>(IList<T> output)
            where T : IComponent
        {
            var components = this.LookUp.Search(typeof(T));
            for (var i = 0; i < components.Count; i++)
            {
                output.Add((T)components[i].Component);
            }
        }

        public bool HasComponent<T>(Entity entity)
            where T : IComponent
        {
            var components = this.LookUp.Search(entity);
            for (var i = 0; i < components.Count; i++)
            {
                if (typeof(T).IsAssignableFrom(components[i].Component.GetType()))
                {
                    return true;
                }
            }
            return false;
        }

        public void GetEntities<T>(IList<Entity> output)
            where T : IComponent
        {
            var components = this.LookUp.Search(typeof(T));
            for (var i = 0; i < components.Count; i++)
            {
                output.Add(components[i].Entity);
            }
        }        
    }
}
