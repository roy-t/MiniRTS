using System.Collections.Generic;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Containers;

namespace MiniEngine.UI.Utilities
{
    public sealed class ComponentSearcher
    {
        private readonly IList<IComponentContainer> Containers;

        public ComponentSearcher(IList<IComponentContainer> containers)
        {
            this.Containers = containers;
        }

        public void GetComponents(Entity entity, List<IComponent> target)
        {
            for (var c = 0; c < this.Containers.Count; c++)
            {
                var container = this.Containers[c];

                for(var i = 0; i < container.Count; i++)
                {
                    var component = container[i];
                    if(component.Entity == entity)
                    {
                        target.Add(component);
                    }
                }
            }
        }

        public IComponentContainer GetContainer(IComponent component)
        {
            for(var i = 0; i < this.Containers.Count; i++)
            {
                var container = this.Containers[i];
                
                if(container.GetComponentType() == component.GetType())
                {
                    return container;
                }
            }

            throw new KeyNotFoundException($"Could not find container for type: {component.GetType().Name}");
        }
    }
}
