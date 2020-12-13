using Microsoft.Xna.Framework;
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

                bounds.BoundingBox = TransformBoundingBox(bounds.NeutralBoundingBox, pose.Matrix);
                bounds.IsInView = perspectiveCamera.Frustum.Intersects(bounds.BoundingBox);
            }
        }

        public static BoundingBox TransformBoundingBox(BoundingBox boundingBox, Matrix world)
        {
            var xa = world.Right * boundingBox.Min.X;
            var xb = world.Right * boundingBox.Max.X;

            var ya = world.Up * boundingBox.Min.Y;
            var yb = world.Up * boundingBox.Max.Y;

            var za = world.Backward * boundingBox.Min.Z;
            var zb = world.Backward * boundingBox.Max.Z;

            return new BoundingBox(
                Vector3.Min(xa, xb) + Vector3.Min(ya, yb) + Vector3.Min(za, zb) + world.Translation,
                Vector3.Max(xa, xb) + Vector3.Max(ya, yb) + Vector3.Max(za, zb) + world.Translation
            );
        }
    }
}
