using System;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryComponent : AComponent, IDisposable
    {
        public GeometryComponent(Entity entity, GeometryModel model, Material material)
            : base(entity)
        {
            this.Geometry = model;
            this.Material = material;
        }

        public GeometryModel Geometry { get; }
        public Material Material { get; }

        public void Dispose() => this.Geometry.Dispose();
    }
}
