using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Geometry
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GeometryVertex : IVertexType
    {
        private static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
           new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
           new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
           new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;

        public GeometryVertex(Vector3 position, Vector3 normal, Vector2 texture)
        {
            this.Position = position;
            this.Normal = normal;
            this.Texture = texture;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
