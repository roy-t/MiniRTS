using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.VertexTypes;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class GeometryFactory : AComponentFactory<Geometry>
    {
        private readonly IComponentContainer<Bounds> Bounds;

        public GeometryFactory(GraphicsDevice device,
            IComponentContainer<Geometry> container,
            IComponentContainer<Bounds> bounds)
            : base(device, container)
        {
            this.Bounds = bounds;
        }

        public Geometry Construct(Entity entity, GBufferVertex[] vertices, short[] indices, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            var geometry = new Geometry(entity, vertices, indices, primitiveType);
            this.Container.Add(geometry);

            BoundaryComputer.ComputeExtremes(geometry, out var min, out var max);
            var bounds = new Bounds(entity, min, max);
            this.Bounds.Add(bounds);

            return geometry;
        }

        public override void Deconstruct(Entity entity)
        {
            this.Bounds.Remove(entity);
            base.Deconstruct(entity);
        }
    }
}
