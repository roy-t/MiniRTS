using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Geometry;

namespace MiniEngine.Graphics.Visibility
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
