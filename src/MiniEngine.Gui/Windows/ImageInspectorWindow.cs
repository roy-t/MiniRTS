using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Utilities;
using Serilog;

namespace MiniEngine.Gui.Windows
{
    [Service]
    public sealed class ImageInspectorWindow : IWindow
    {
        private readonly GraphicsDevice Device;
        private readonly OpaqueEffect Effect;
        private readonly PostProcessTriangle Triangle;
        private readonly ILogger Logger;
        private readonly ImGuiRenderer GuiRenderer;
        private readonly List<Texture2D> Textures;

        private int selected;

        public ImageInspectorWindow(ImGuiRenderer guiRenderer, ILogger logger, GraphicsDevice device, OpaqueEffect effect, PostProcessTriangle triangle)
        {
            this.GuiRenderer = guiRenderer;
            this.Logger = logger;
            this.Device = device;
            this.Effect = effect;
            this.Triangle = triangle;

            this.Textures = new List<Texture2D>();
        }

        public string Name => "Image Inspector";

        public string ImageEditor { get; set; } = @"C:\Program Files\paint.net\PaintDotNet.exe";

        public bool AllowTransparency => false;

        public void RenderContents()
        {
            this.Textures.RemoveAll(x => x.IsDisposed);
            if (this.Textures.Count > 0)
            {
                this.selected = Math.Clamp(this.selected, 0, this.Textures.Count - 1);
                var texture = this.Textures[this.selected];

                var id = this.GetTextureId(this.GuiRenderer, texture);
                this.ShowImage(id, texture, this.Logger);
            }
        }

        private IntPtr GetTextureId(ImGuiRenderer guiRenderer, Texture2D texture)
        {
            if (texture.Tag == null)
            {
                texture.Tag = guiRenderer.BindTexture(texture);
            }

            return (IntPtr)texture.Tag;
        }

        private void ShowImage(IntPtr id, Texture2D texture, ILogger logger)
        {
            if (!string.IsNullOrWhiteSpace(texture.Name))
            {
                ImGui.Text(texture.Name);
            }
            ImGui.Text($"{texture.Width}x{texture.Height}x{texture.LevelCount} @ {texture.Format}");

            var windowSize = ImageUtilities.GetWindowSize();
            var imageSize = ImageUtilities.FitToBounds(texture.Width, texture.Height, windowSize.X, windowSize.Y);
            ImGui.Image(id, imageSize);

            if (ImGui.Button("Edit"))
            {
                var temp = Path.Join(Path.GetTempPath(), $"{DateTime.Now.Ticks}.png");

                try
                {
                    this.SaveTexture(temp, texture);
                    Process.Start(this.ImageEditor, temp);
                }
                catch (Exception ex)
                {
                    logger.Warning(ex, "Could not save or open file {@file}", temp);
                }
            }
            ImGui.SameLine();

            if (ImGui.ArrowButton("InspectorLeft", ImGuiDir.Left))
            {
                --this.selected;
            }

            ImGui.SameLine();
            ImGui.Text($"{this.selected + 1}/{this.Textures.Count}");

            ImGui.SameLine();
            if (ImGui.ArrowButton("InspectorRight", ImGuiDir.Right))
            {
                ++this.selected;
            }

            ImGui.SameLine();
            if (ImGui.Button("Remove"))
            {
                this.Textures.RemoveAt(this.selected);
            }
        }

        private void SaveTexture(string file, Texture2D texture)
        {
            using var renderTarget = new RenderTarget2D(this.Device, texture.Width, texture.Height, false, SurfaceFormat.HalfVector4, DepthFormat.None);
            this.Device.SetRenderTarget(renderTarget);
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Effect.Texture = texture;
            this.Effect.Apply();

            this.Triangle.Render(this.Device);

            this.Device.SetRenderTarget(null);

            using var stream = File.Create(file);
            renderTarget.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
        }

        public void SetImage(Texture2D texture)
        {
            this.Textures.RemoveAll(x => x.IsDisposed);
            this.Textures.Add(texture);
            this.selected = this.Textures.Count - 1;
        }
    }
}
