using Microsoft.Xna.Framework;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Visibility
{
    public sealed class Pose
    {
        public Pose(Entity entity, GeometryModel model, Matrix transform)
        {
            this.Entity = entity;
            this.Model = model;
            this.Transform = transform;
        }

        public Entity Entity { get; }
        public GeometryModel Model { get; }
        public Matrix Transform { get; }
    }
}
