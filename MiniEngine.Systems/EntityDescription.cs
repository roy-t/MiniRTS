using MiniEngine.Systems.Components;
using System.Collections.Generic;

namespace MiniEngine.Systems
{
    public sealed class EntityDescription
    {
        public EntityDescription(Entity entity, IReadOnlyList<ComponentDescription> components)            
        {
            this.Entity = entity;
            this.Components = components;
        }

        public Entity Entity { get; }
        public int ComponentCount => this.Components.Count;
        public IReadOnlyList<ComponentDescription> Components { get; }
    }
}
