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
            this.Transform = transform;
            this.RenderService = renderService;
        }

        public Entity Entity { get; }
        public BoundingSphere Bounds { get; set; }
        public Transform Transform { get; set; }
        public IRenderService RenderService { get; }
    }
}
