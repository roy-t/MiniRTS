using Microsoft.Xna.Framework;

namespace MiniEngine.ContentPipeline.Shared
{
    public sealed class Pose
    {
        public Pose(GeometryData geometry, Material material, Matrix transform)
        {
            this.Geometry = geometry;
            this.Material = material;
            this.Transform = transform;
        }

        public GeometryData Geometry { get; }
        public Material Material { get; }
        public Matrix Transform { get; }
    }
}
