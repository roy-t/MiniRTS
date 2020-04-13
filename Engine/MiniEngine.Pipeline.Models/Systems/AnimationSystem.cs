using System;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Models.Systems
{
    // TODO: once there is more than one animation, we should let each animation have its own system
    // which gets the model component on the entity and updates that
    public sealed class AnimationSystem : IUpdatableSystem
    {
        private readonly IComponentContainer<AAnimation> Animations;
        private readonly IComponentContainer<OpaqueModel> Models;

        public AnimationSystem(IComponentContainer<AAnimation> animations, IComponentContainer<OpaqueModel> models)
        {
            this.Animations = animations;
            this.Models = models;
        }

        public void Update(PerspectiveCamera _, Seconds elapsed)
        {
            for (var i = 0; i < this.Animations.Count; i++)
            {
                var animation = this.Animations[i];
                var model = this.Models.Get(animation.Entity);

                animation.Update(elapsed);

                Array.Copy(animation.SkinTransforms, 0, model.SkinTransforms, 0, animation.SkinTransforms.Length);
            }
        }
    }
}
