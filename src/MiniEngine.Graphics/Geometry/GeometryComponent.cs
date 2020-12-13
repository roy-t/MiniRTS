using System;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryComponent : AComponent, IDisposable
    {
        public GeometryComponent(Entity entity, GeometryData geometry, Material material)
            : base(entity)
        {
            this.Geometry = geometry;
            this.Material = material;
        }
        public GeometryData Geometry { get; }
        public Material Material { get; }

        public void Dispose() => this.Geometry.Dispose();
    }
}
