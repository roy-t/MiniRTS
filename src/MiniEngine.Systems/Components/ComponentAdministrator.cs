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


        public T GetComponent<T>(Entity entity)
            where T : AComponent
        {
            var store = this.ContainerStore.GetContainer<T>();
            var component = store.Get(entity);

            return component;
        }

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

        public IReadOnlyList<T> GetComponents<T>()
            where T : AComponent
        {
            var components = new List<T>();

            var container = this.ContainerStore.GetContainer<T>();
            for (var i = 0; i < container.All.Count; i++)
            {
                var component = container.All[i];
                components.Add(component);
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
