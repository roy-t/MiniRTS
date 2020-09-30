using System;
using System.Collections.Generic;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Components
{
    [Service]
    public sealed class ComponentAdministrator
    {
        private readonly Dictionary<Type, IComponentContainer> ComponentContainers;

        public ComponentAdministrator(IEnumerable<IComponentContainer> componentContainers)
        {
            this.ComponentContainers = new Dictionary<Type, IComponentContainer>();
            foreach (var componentContainer in componentContainers)
            {
                this.ComponentContainers.Add(componentContainer.ComponentType, componentContainer);
            }
        }

        public void Add<T>(T component)
            where T : AComponent => this.ComponentContainers[typeof(T)].AddUnsafe(component);
    }
}
