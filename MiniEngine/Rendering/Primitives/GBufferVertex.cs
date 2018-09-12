using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Primitives
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GBufferVertex : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration(new []
        {
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 0),
            new VertexElement(16, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
            new VertexElement(48, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0)
        });

        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector3 BiNormal;
        public Vector3 Tangent;

        public GBufferVertex(Vector3 position)
        {
            this.Position = new Vector4(position, 1);
            this.Normal = Vector3.UnitZ;
            this.TextureCoordinate = Vector2.Zero;
            this.BiNormal = Vector3.UnitX;
            this.Tangent = Vector3.UnitY;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
