using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Gui
{
    static class GuiVertexDeclaration
    {
        public static readonly VertexDeclaration Declaration;

        public static readonly int Size;

        static GuiVertexDeclaration()
        {
            unsafe { Size = sizeof(ImDrawVert); }

            Declaration = new VertexDeclaration(
                Size,

                // Position
                new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),

                // UV
                new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),

                // Color
                new VertexElement(16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
            );
        }
    }
}
