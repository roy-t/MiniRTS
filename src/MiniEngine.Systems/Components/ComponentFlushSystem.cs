using System.Collections.Generic;
using System.Linq;
using MiniEngine.Configuration;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Systems.Components
{
    [System]
    public partial class ComponentFlushSystem : ISystem
    {
        private readonly IReadOnlyList<IComponentContainer> Containers;

        public ComponentFlushSystem(IEnumerable<IComponentContainer> containers)
        {
            this.Containers = containers.ToList();
        }

        public void OnSet()
        {
        }

        [Process]
        public void Process()
        {
            for (var i = 0; i < this.Containers.Count; i++)
            {
                this.Containers[i].Flush();
            }
        }
    }
}
