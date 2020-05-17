using System.Collections.Generic;

namespace MiniEngine.Systems.Components
{
    public sealed class Parent : IComponent
    {
        public Parent(Entity entity)
        {
            this.Entity = entity;
            this.Children = new List<Entity>(0);
        }

        public Entity Entity { get; }

        public List<Entity> Children { get; }
    }
}
