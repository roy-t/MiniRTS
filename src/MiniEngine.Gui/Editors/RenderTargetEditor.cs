using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class RenderTargetEditor : AEditor<RenderTarget2D>
    {
        private readonly ImGuiRenderer GuiRenderer;

        public RenderTargetEditor(ImGuiRenderer guiRenderer)
        {
            this.GuiRenderer = guiRenderer;
        }

        public override bool Draw(string name, Func<RenderTarget2D> get, Action<RenderTarget2D> set)
        {
            var renderTarget = get();
            this.DrawRenderTarget(name, renderTarget);

            return false;
        }

        private void DrawRenderTarget(string name, RenderTarget2D renderTarget)
        {
            if (renderTarget.Tag == null)
            {
                renderTarget.Tag = this.GuiRenderer.BindTexture(renderTarget);
            }
            ImGui.Text($"{name} : {renderTarget.Format} ({renderTarget.Width}x{renderTarget.Height}x{renderTarget.LevelCount})");

            var maxWidth = Math.Min(ImGui.CalcItemWidth(), 1024);
            var size = ImageUtilities.FitToBounds(renderTarget.Width, renderTarget.Height, maxWidth, 1024);
            ImGui.Image((IntPtr)renderTarget.Tag, size, Vector2.Zero, Vector2.One);
        }
    }
}
