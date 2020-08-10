using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            : this(new Vector4(position, 1.0f), Vector3.Backward, Vector3.Right, Vector3.Up, Vector2.Zero) { }

        public GBufferVertex(Vector3 position, Vector2 textureCoordinate)
            : this(new Vector4(position, 1.0f), Vector3.Backward, Vector3.Right, Vector3.Up, textureCoordinate) { }

        public GBufferVertex(Vector4 position, float u = 0, float v = 0)
            : this(position, Vector3.Backward, Vector3.Right, Vector3.Up, new Vector2(u, v)) { }


        public GBufferVertex(Vector3 position, Vector3 normal, Vector3 tangent, Vector3 biNormal)
            : this(new Vector4(position, 1), normal, tangent, biNormal, Vector2.Zero) { }

        public GBufferVertex(Vector4 position, Vector3 normal, Vector3 tangent, Vector3 biNormal, Vector2 textureCoordinate)
        {
            this.Position = position;
            this.Normal = normal;
            this.BiNormal = biNormal;
            this.Tangent = tangent;
            this.TextureCoordinate = textureCoordinate;
        }

        public Vector3 PositionXYZ => new Vector3(this.Position.X, this.Position.Y, this.Position.Z) / this.Position.W;

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}