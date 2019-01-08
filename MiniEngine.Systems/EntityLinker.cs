using MiniEngine.Systems.Components;
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
            foreach (var component in components)
            {
                output.Add(component.Component);
            }
        }

        public void GetComponents<T>(IList<T> output)
            where T : IComponent
        {
            var components = this.LookUp.Search(typeof(T));
            foreach (var component in components)
            {
                output.Add((T)component.Component);
            }
        }

        public void GetEntities<T>(IList<Entity> output)
            where T : IComponent
        {
            var components = this.LookUp.Search(typeof(T));
            foreach(var component in components)
            {
                output.Add(component.Entity);
            }
        }
    }
}
