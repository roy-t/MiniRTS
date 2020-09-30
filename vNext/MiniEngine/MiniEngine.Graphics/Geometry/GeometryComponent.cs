using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryComponent : AComponent
    {
        public GeometryComponent(Entity entity, GeometryVertex[] vertices, int[] indices)
            : base(entity)
        {
            this.Vertices = vertices;
            this.Indices = indices;
        }

        public GeometryVertex[] Vertices { get; }
        public int[] Indices { get; }
    }
}
