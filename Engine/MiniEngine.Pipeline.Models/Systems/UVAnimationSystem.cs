using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Models.Systems
{
    public sealed class UVAnimationSystem : IUpdatableSystem
    {
        private readonly IComponentContainer<UVAnimation> Animations;
        private readonly IComponentContainer<OpaqueModel> Models;

        public UVAnimationSystem(IComponentContainer<UVAnimation> animations, IComponentContainer<OpaqueModel> models)
        {
            this.Animations = animations;
            this.Models = models;
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            for (var i = 0; i < this.Animations.Count; i++)
            {
                var animation = this.Animations[i];
                var model = this.Models.Get(animation.Entity);

                for (var offsetIndex = 0; offsetIndex < animation.MeshUVOffsets.Length; offsetIndex++)
                {
                    var offset = animation.MeshUVOffsets[offsetIndex];
                    model.UVOffsets[offset.MeshIndex] = offset.UVOffset;
                }
            }
        }
    }
}
