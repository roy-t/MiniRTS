using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace MiniEngine.Primitives.VertexTypes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GBufferVertex : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
            new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
            new VertexElement(48, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0));

        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector3 BiNormal;
        public Vector3 Tangent;

        public GBufferVertex(Vector3 position)
            : this(position, Vector2.Zero)
        {
        }

        public GBufferVertex(Vector3 position, Vector2 textureCoordinate)
        {
            this.Position = new Vector4(position, 1);
            this.Normal = Vector3.Forward;
            this.TextureCoordinate = textureCoordinate;
            this.BiNormal = Vector3.Up;
            this.Tangent = Vector3.Right;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}