using System;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryComponent : AComponent, IDisposable
    {
        public GeometryComponent(Entity entity, Geometry geometry, Material material)
            : base(entity)
        {
            this.Geometry = geometry;
            this.Material = material;
        }
        public Geometry Geometry { get; }
        public Material Material { get; }

        public void Dispose() => this.Geometry.Dispose();
    }
}
