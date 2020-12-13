using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Systems.Factories
{
    public sealed class ParentFactory : AComponentFactory<Parent>
    {
        public ParentFactory(GraphicsDevice device, IComponentContainer<Parent> container)
            : base(device, container) { }

        public Parent Construct(Entity entity, params Entity[] children)
        {
            var parent = new Parent(entity);
            parent.Children.AddRange(children);

            this.Container.Add(parent);
            return parent;
        }
    }
}
