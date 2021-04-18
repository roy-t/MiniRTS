using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Visibility
{
    public sealed class Pose
    {
        public Pose(Entity entity, BoundingSphere bounds, Transform transform, IRenderService renderService)
        {
            this.Entity = entity;
            this.Bounds = bounds;
            this.RenderService = renderService;
            this.Transform = transform;
        }

        public Entity Entity { get; }
        public BoundingSphere Bounds { get; }
        public IRenderService RenderService { get; }
        public Transform Transform { get; set; }
    }
}
