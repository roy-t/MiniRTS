using System;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Components
{
    [Service]
    public sealed class ContainerStore
    {
        private readonly IReadOnlyList<IComponentContainer> Containers;
        private readonly Dictionary<Type, IComponentContainer> ContainersByType;

        public ContainerStore(IEnumerable<IComponentContainer> containers)
        {
            this.Containers = containers.Distinct().ToList();
            this.ContainersByType = this.Containers.ToDictionary(x => x.ComponentType);
        }

        public IReadOnlyList<IComponentContainer> GetAllContainers()
            => this.Containers;

        public IComponentContainer<T> GetContainer<T>()
            where T : AComponent
        {
            var key = typeof(T);
            return this.ContainersByType[key].Specialize<T>();
        }
    }
}
