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

        public CarAnimation Construct(Entity entity, AModel model)
        {
            var animation = new CarAnimation(entity, model);
            this.Container.Add(entity, animation);

            return animation;
        }
    }
}
