using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Basics.Systems
{
    public sealed class BoundsSystem : IUpdatableSystem
    {
        private readonly IComponentContainer<Bounds> Bounds;
        private readonly IComponentContainer<Pose> Poses;

        public BoundsSystem(IComponentContainer<Bounds> bounds, IComponentContainer<Pose> poses)
        {
            this.Bounds = bounds;
            this.Poses = poses;
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            for (var i = 0; i < this.Bounds.Count; i++)
            {
                var bounds = this.Bounds[i];
                var pose = this.Poses.Get(bounds.Entity);

                bounds.BoundingSphere = bounds.NeutralBoundingSphere.Transform(pose.Matrix);
                bounds.IsInView = perspectiveCamera.Frustum.Intersects(bounds.BoundingSphere);
            }
        }
    }
}
