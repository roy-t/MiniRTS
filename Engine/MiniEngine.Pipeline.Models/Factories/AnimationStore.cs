using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class AnimationStore : AComponentFactory<AAnimation>
    {
        public AnimationStore(GraphicsDevice _, IComponentContainer<AAnimation> container)
            : base(_, container) { }

        public void Store(AAnimation animation) => this.Container.Add(animation);
    }
}
