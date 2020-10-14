using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryComponent : AComponent
    {
        public GeometryComponent(Entity entity, GeometryVertex[] vertices, int[] indices, Material material)
            : base(entity)
        {
            this.Vertices = vertices;
            this.Indices = indices;
            this.Primitives = this.Indices.Length / 3;

            this.Material = material;
        }

        public GeometryVertex[] Vertices { get; }
        public int[] Indices { get; }

        public int Primitives { get; }

        public Material Material { get; }
    }
}
