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

        public void Add<T, U>(T componentA, U componentB)
            where T : AComponent
            where U : AComponent
        {
            this.ContainerStore.GetContainer<T>().Add(componentA);
            this.ContainerStore.GetContainer<U>().Add(componentB);
        }

        public void Add<T, U, V>(T componentA, U componentB, V componentC)
            where T : AComponent
            where U : AComponent
            where V : AComponent
        {
            this.ContainerStore.GetContainer<T>().Add(componentA);
            this.ContainerStore.GetContainer<U>().Add(componentB);
            this.ContainerStore.GetContainer<V>().Add(componentC);
        }

        public void Add<T, U, V, W>(T componentA, U componentB, V componentC, W componentD)
            where T : AComponent
            where U : AComponent
            where V : AComponent
            where W : AComponent
        {
            this.ContainerStore.GetContainer<T>().Add(componentA);
            this.ContainerStore.GetContainer<U>().Add(componentB);
            this.ContainerStore.GetContainer<V>().Add(componentC);
            this.ContainerStore.GetContainer<W>().Add(componentD);
        }

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
