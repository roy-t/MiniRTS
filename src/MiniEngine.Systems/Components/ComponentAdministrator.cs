using System.Collections.Generic;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Components
{
    [Service]
    public sealed class ComponentAdministrator
    {
        private readonly ContainerStore ContainerStore;

        public ComponentAdministrator(ContainerStore containerStore)
        {
            this.ContainerStore = containerStore;
        }

        public void Add<T>(T component)
            where T : AComponent
            => this.ContainerStore.GetContainer<T>().Add(component);

        public IReadOnlyList<AComponent> GetComponents(Entity entity)
        {
            var components = new List<AComponent>();

            var containers = this.ContainerStore.GetAllContainers();
            for (var i = 0; i < containers.Count; i++)
            {
                var container = containers[i];
                if (container.Contains(entity))
                {
                    components.Add(container.GetComponent(entity));
                }
            }

            return components;
        }

        public void MarkForRemoval(Entity entity)
        {
            var containers = this.ContainerStore.GetAllContainers();
            for (var i = 0; i < containers.Count; i++)
            {
                var container = containers[i];
                if (container.Contains(entity))
                {
                    container.GetComponent(entity).ChangeState.Remove();
                }
            }
        }
    }
}
