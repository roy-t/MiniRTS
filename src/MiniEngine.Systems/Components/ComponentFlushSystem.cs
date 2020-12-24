using MiniEngine.Configuration;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Systems.Components
{
    [System]
    public partial class ComponentFlushSystem : ISystem
    {
        private readonly ContainerStore ContainerStore;

        public ComponentFlushSystem(ContainerStore containerStore)
        {
            this.ContainerStore = containerStore;
        }

        public void OnSet()
        {
        }

        [Process]
        public void Process()
        {
            var containers = this.ContainerStore.GetAllContainers();
            for (var i = 0; i < containers.Count; i++)
            {
                containers[i].Flush();
            }
        }
    }
}
