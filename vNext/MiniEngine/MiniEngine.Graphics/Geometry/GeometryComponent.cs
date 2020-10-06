using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryComponent : AComponent
    {
        public GeometryComponent(Entity entity, GeometryVertex[] vertices, int[] indices, Texture2D diffuse, Texture2D normal)
            : base(entity)
        {
            this.Vertices = vertices;
            this.Indices = indices;
            this.Diffuse = diffuse;
            this.Normal = normal;
        }

        public GeometryVertex[] Vertices { get; }
        public int[] Indices { get; }
        public Texture2D Diffuse { get; set; }

        public Texture2D Normal { get; set; }
    }
}
