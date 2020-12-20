using System;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Components
{
    [Service]
    public sealed class ComponentAdministrator
    {
        private readonly Dictionary<Type, IComponentContainer> ComponentContainers;

        public ComponentAdministrator(IEnumerable<IComponentContainer> componentContainers)
        {
            this.ComponentContainers = componentContainers.ToDictionary(x => x.ComponentType);
        }

        public void Add<T>(T component)
            where T : AComponent
            => this.ComponentContainers[typeof(T)].Specialize<T>().Add(component);

        public IReadOnlyList<AComponent> GetComponents(Entity entity)
        {
            var components = new List<AComponent>();
            foreach (var container in this.ComponentContainers.Values)
            {
                if (container.Contains(entity))
                {
                    components.Add(container.GetComponent(entity));
                }
            }

            return components;
        }

        public void MarkForRemoval(Entity entity)
        {
            foreach (var container in this.ComponentContainers.Values)
            {
                if (container.Contains(entity))
                {
                    container.GetComponent(entity).ChangeState.Remove();
                }
            }
        }
    }
}
