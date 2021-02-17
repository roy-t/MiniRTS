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

        private const float MaxPreviewSize = 500.0f;
        private const float MinPreviewSize = 19.0f;
        private readonly ImGuiRenderer GuiRenderer;
        private readonly ImageInspectorWindow ImageInspector;

        public TextureTool(ImGuiRenderer guiRenderer, ImageInspectorWindow imageInspector)
        {
            this.GuiRenderer = guiRenderer;
            this.ImageInspector = imageInspector;
        }

        public override bool HeaderValue(ref Texture2D value, ToolState tool)
        {
            this.GuiRenderer.BindTexture(value);

            var previewSize = new Vector2(Math.Clamp(tool.X, MinPreviewSize, MaxPreviewSize), Math.Clamp(tool.Y, MinPreviewSize, MaxPreviewSize));

            var size = ImageUtilities.FitToBounds(value.Width, value.Height, previewSize.X, previewSize.Y);
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

        public override ToolState Configure(ToolState tool)
        {
            var size = new Vector2(tool.X, tool.Y);

            if (ImGui.InputFloat2("Max preview size", ref size))
            {
                tool.X = Math.Clamp(size.X, MinPreviewSize, MaxPreviewSize);
                tool.Y = Math.Clamp(size.Y, MinPreviewSize, MaxPreviewSize);
            }
            return tool;
        }
    }
}
