using Microsoft.Xna.Framework;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.Graphics.Visibility
{
    public sealed class Pose
    {
        public Pose(GeometryModel model, Matrix transform)
        {
            this.Model = model;
            this.Transform = transform;
        }

        public GeometryModel Model { get; }
        public Matrix Transform { get; }
    }
}
