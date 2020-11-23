using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class Pose
    {
        public Pose(Geometry geometry, Material material, Matrix transform)
        {
            this.Geometry = geometry;
            this.Material = material;
            this.Transform = transform;
        }

        public Geometry Geometry { get; }
        public Material Material { get; }
        public Matrix Transform { get; }
    }
}
