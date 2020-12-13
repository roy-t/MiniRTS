using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Basics.Factories
{
    public sealed class SelectionHitboxFactory : AComponentFactory<SelectionHitbox>
    {
        public SelectionHitboxFactory(GraphicsDevice _, IComponentContainer<SelectionHitbox> container)
            : base(_, container) { }

        public SelectionHitbox Construct(Entity entity, float width, float height, float depth)
        {
            var selectionHitBox = new SelectionHitbox(entity, width, height, depth);
            this.Container.Add(selectionHitBox);

            return selectionHitBox;
        }
    }
}
