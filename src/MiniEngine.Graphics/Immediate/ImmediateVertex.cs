using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Gui
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ImmediateVertex : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
            new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
        );

        public Vector2 Position;
        public Vector2 Texture;
        public Color Color;

        public ImmediateVertex(Vector2 position, Vector2 texture, Color color)
        {
            this.Position = position;
            this.Texture = texture;
            this.Color = color;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
