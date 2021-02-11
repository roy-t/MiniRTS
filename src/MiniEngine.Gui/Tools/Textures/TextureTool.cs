using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Gui.Windows;

namespace MiniEngine.Gui.Tools.Textures
{
    [Service]
    public sealed class TextureTool : ATool<Texture2D>
    {
        public override string Name => "Texture";
        private readonly ImGuiRenderer GuiRenderer;
        private readonly ImageInspectorWindow ImageInspector;

        public TextureTool(ImGuiRenderer guiRenderer, ImageInspectorWindow imageInspector)
        {
            this.GuiRenderer = guiRenderer;
            this.ImageInspector = imageInspector;
        }

        public override bool HeaderValue(ref Texture2D value, ToolState tool)
        {
            if (value.Tag == null)
            {
                value.Tag = this.GuiRenderer.BindTexture(value);
            }

            var size = ImageUtilities.FitToBounds(value.Width, value.Height, 19, 19);
            if (ImGui.ImageButton((IntPtr)value.Tag, size, Vector2.Zero, Vector2.One, 0))
            {
                this.ImageInspector.SetImage(value);
            }

            return false;
        }

        public override bool Details(ref Texture2D value, ToolState tool)
        {
            ToolUtils.TextRow("Width", $"{value.Width}");
            ToolUtils.TextRow("Height", $"{value.Width}");
            ToolUtils.TextRow("Format", $"{value.Format}");

            return false;
        }
    }
}
