using System.Collections.Generic;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Systems
{
    public sealed class EntityLinker2
    {
        private readonly Dictionary<int, IComponentContainer> Containers;

        public EntityLinker2(List<IComponentContainer> containers)
        {
            this.Containers = new Dictionary<int, IComponentContainer>();

            for(var i = 0; i < containers.Count; i++)
            {
                var container = containers[i];
                var hash = container.GetComponentType().GetHashCode();

                this.Containers.Add(hash, container);
            }
        }


        //public void GetComponents
    }
}
