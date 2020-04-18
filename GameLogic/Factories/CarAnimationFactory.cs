using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.GameLogic.Factories
{
    public sealed class CarAnimationFactory : AComponentFactory<AAnimation>
    {
        public CarAnimationFactory(GraphicsDevice _, IComponentContainer<AAnimation> container)
            : base(_, container) { }

        public CarAnimation Construct(Entity entity, SkinningData skinningData)
        {
            var animation = new CarAnimation(entity, skinningData);
            this.Container.Add(animation);

            return animation;
        }
    }
}
