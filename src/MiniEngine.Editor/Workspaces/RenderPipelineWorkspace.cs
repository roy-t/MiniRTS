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
        private readonly PropertyInspector PropertyInspector;
        private readonly List<(string, Texture2D)> Buffers;
        private readonly ImGuiRenderer Renderer;


        public RenderPipelineWorkspace(ImGuiRenderer renderer, FrameService frameService, PropertyInspector propertyInspector)
        {
            this.Renderer = renderer;
            this.Buffers = new List<(string, Texture2D)>();
            this.PropertyInspector = propertyInspector;

            this.RegisterTexture(nameof(frameService.GBuffer.Albedo), frameService.GBuffer.Albedo);
            this.RegisterTexture(nameof(frameService.GBuffer.Normal), frameService.GBuffer.Normal);
            this.RegisterTexture(nameof(frameService.GBuffer.Depth), frameService.GBuffer.Depth);
            this.RegisterTexture(nameof(frameService.GBuffer.Material), frameService.GBuffer.Material);
            this.RegisterTexture(nameof(frameService.LBuffer.Light), frameService.LBuffer.Light);
            this.RegisterTexture(nameof(frameService.PBuffer.ToneMap), frameService.PBuffer.ToneMap);
            this.RegisterTexture(nameof(frameService.TBuffer.Albedo), frameService.TBuffer.Albedo);
            this.RegisterTexture(nameof(frameService.TBuffer.Weights), frameService.TBuffer.Weights);
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
                var windowSize = ImageUtilities.GetWindowSize();

                foreach (var (name, texture) in this.Buffers)
                {
                    var imageSize = ImageUtilities.FitToBounds(texture.Width, texture.Height, windowSize.X, windowSize.Y);
                    ImGui.Text(name);
                    ImGui.Image((IntPtr)texture.Tag, imageSize);
                    if (ImGui.Button($"Inspect {name}"))
                    {
                        this.PropertyInspector.Textures.Add(texture);
                    }
                }

                ImGui.End();
            }
            ImGui.PopStyleColor(2);
        }
    }
}
