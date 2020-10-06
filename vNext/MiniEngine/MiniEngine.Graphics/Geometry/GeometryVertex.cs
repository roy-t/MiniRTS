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
           new VertexElement(0 * 4, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
           new VertexElement(3 * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
           new VertexElement(5 * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
           new VertexElement(8 * 4, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
           new VertexElement(11 * 4, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0)
        );

        public Vector3 Position;
        public Vector2 Texture;
        public Vector3 Normal;
        public Vector3 Binormal;
        public Vector3 Tangent;

        public GeometryVertex(Vector3 position, Vector2 texture, Vector3 normal, Vector3 binormal, Vector3 tangent)
        {
            this.Position = position;
            this.Texture = texture;
            this.Normal = normal;
            this.Binormal = binormal;
            this.Tangent = tangent;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
