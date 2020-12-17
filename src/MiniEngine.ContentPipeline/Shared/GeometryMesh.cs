using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.ContentPipeline.Shared
{
    public sealed class GeometryMesh : IDisposable
    {
        public GeometryMesh(GeometryData geometry, Material material, Matrix offset)
        {
            this.Geometry = geometry;
            this.Material = material;
            this.Offset = offset;
        }

        public GeometryData Geometry { get; }
        public Material Material { get; }
        public Matrix Offset { get; }

        public void Dispose()
            => this.Geometry.Dispose();
    }
}
