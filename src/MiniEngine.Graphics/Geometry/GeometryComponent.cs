using System;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryComponent : AComponent, IDisposable
    {
        public GeometryComponent(Entity entity, GeometryModel model)
            : base(entity)
        {
            this.Geometry = model;
        }

        public GeometryModel Geometry { get; }

        public void Dispose() => this.Geometry.Dispose();
    }
}
