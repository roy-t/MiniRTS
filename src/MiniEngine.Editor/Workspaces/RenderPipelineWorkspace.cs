using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Gui;

namespace MiniEngine.Editor.Workspaces
{
    [Service]
    public sealed class RenderPipelineWorkspace : IWorkspace
    {
        private readonly List<(string, Texture2D)> Buffers;
        private readonly ImGuiRenderer Renderer;

        public RenderPipelineWorkspace(ImGuiRenderer renderer, FrameService frameService)
        {
            this.Renderer = renderer;
            this.Buffers = new List<(string, Texture2D)>();

            this.RegisterTexture(nameof(frameService.GBuffer.Albedo), frameService.GBuffer.Albedo);
            this.RegisterTexture(nameof(frameService.GBuffer.Normal), frameService.GBuffer.Normal);
            this.RegisterTexture(nameof(frameService.GBuffer.Material), frameService.GBuffer.Material);
            this.RegisterTexture(nameof(frameService.LBuffer.Light), frameService.LBuffer.Light);
            this.RegisterTexture(nameof(frameService.LBuffer.LightPostProcess), frameService.LBuffer.LightPostProcess);
            this.RegisterTexture(nameof(frameService.PBuffer.ToneMap), frameService.PBuffer.ToneMap);
        }

        private void RegisterTexture(string name, Texture2D texture)
        {
            texture.Tag = this.Renderer.BindTexture(texture);
            this.Buffers.Add((name, texture));
        }

        public void RenderWindows()
        {
            ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Black.ToVector4());
            ImGui.PushStyleColor(ImGuiCol.WindowBg, Color.Black.ToVector4());
            if (ImGui.Begin("Buffers"))
            {
                var width = ImGui.GetWindowWidth();
                var height = ImGui.GetWindowHeight() - (ImGui.GetFrameHeightWithSpacing() * 2);

                foreach (var (name, texture) in this.Buffers)
                {
                    var imageSize = ImageUtilities.FitToBounds(texture.Width, texture.Height, width, height);
                    ImGui.Text(name);
                    ImGui.Image((IntPtr)texture.Tag, imageSize);
                }

                ImGui.End();
            }
            ImGui.PopStyleColor(2);
        }
    }
}
