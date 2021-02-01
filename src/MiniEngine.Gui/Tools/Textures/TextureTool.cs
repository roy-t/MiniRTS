using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Textures
{
    [Service]
    public sealed class TextureTool : ATool<Texture2D>
    {
        public override string Name => "Texture";
        private readonly ImGuiRenderer GuiRenderer;

        private IntPtr Active;

        public TextureTool(ImGuiRenderer guiRenderer)
        {
            this.GuiRenderer = guiRenderer;
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
                this.Active = (IntPtr)value.Tag;
            }

            this.TexturePreviewWindow(value);

            return false;
        }

        private void TexturePreviewWindow(Texture2D value)
        {
            if (this.Active == (IntPtr)value.Tag)
            {
                var open = true;

                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 1.0f);
                if (ImGui.Begin("Texture Preview", ref open))
                {

                    var windowSize = ImageUtilities.GetWindowSize();
                    var imageSize = ImageUtilities.FitToBounds(value.Width, value.Height, windowSize.X, windowSize.Y);
                    ImGui.Image((IntPtr)value.Tag, imageSize);

                    if (!open)
                    {
                        this.Active = IntPtr.Zero;
                    }

                    ImGui.End();
                }
                ImGui.PopStyleVar();
            }
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
