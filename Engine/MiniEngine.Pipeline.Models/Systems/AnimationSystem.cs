using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Models.Systems
{
    public sealed class AnimationSystem : IUpdatableSystem
    {
        private readonly IComponentContainer<AAnimation> Animations;

        public AnimationSystem(IComponentContainer<AAnimation> animations)
        {
            this.Animations = animations;
        }

        public void Update(PerspectiveCamera _, Seconds elapsed)
        {
            for (var i = 0; i < this.Animations.Count; i++)
            {
                var animation = this.Animations[i];

                animation.Update(elapsed);
            }
        }
    }
}
