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
    }
}
