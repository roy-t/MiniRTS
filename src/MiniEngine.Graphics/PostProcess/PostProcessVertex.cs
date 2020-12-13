using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.PostProcess
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PostProcessVertex : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
           new VertexElement(0 * 4, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
           new VertexElement(3 * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        public Vector3 Position;
        public Vector2 Texture;

        public PostProcessVertex(Vector3 position, Vector2 texture)
        {
            this.Position = position;
            this.Texture = texture;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
