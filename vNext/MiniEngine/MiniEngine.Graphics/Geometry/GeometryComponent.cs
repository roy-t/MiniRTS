using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryComponent : AComponent
    {
        public GeometryComponent(Entity entity, GeometryVertex[] vertices, int[] indices, Texture2D diffuse)
            : base(entity)
        {
            this.Vertices = vertices;
            this.Indices = indices;
            this.Diffuse = diffuse;
        }

        public GeometryVertex[] Vertices { get; }
        public int[] Indices { get; }
        public Texture2D Diffuse { get; set; }
    }
}
