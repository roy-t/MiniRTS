using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Gui;
using Serilog;

namespace MiniEngine.Editor.Workspaces.Editors
{
    [Service]
    public sealed class ImageInspector
    {
        private readonly PropertyInspector PropertyInspector;
        private readonly ILogger Logger;
        private int selected;
        private bool open;

        public ImageInspector(PropertyInspector propertyInspector, ILogger Logger)
        {
            this.PropertyInspector = propertyInspector;
            this.Logger = Logger;
            this.PropertyInspector.Textures.CollectionChanged += (s, e) =>
            {
                this.open = true;
                this.selected = this.PropertyInspector.Textures.Count - 1;
            };
        }

        public bool IsOpen
        {
            get => this.open;
            set => this.open = value;
        }

        public void Render()
        {
            if (ImGui.Begin("Image Inspector", ref this.open))
            {
                var textures = this.PropertyInspector.Textures.Where(t => t.IsDisposed == false).ToList();
                if (textures.Count > 0)
                {
                    this.selected = Math.Clamp(this.selected, 0, textures.Count - 1);
                    var texture = textures[this.selected];
                    var windowSize = ImageUtilities.GetWindowSize();
                    var textureSize = ImageUtilities.FitToBounds(texture.Width, texture.Height, windowSize.X, windowSize.Y);
                    ImGui.Image((IntPtr)texture.Tag, textureSize);

                    if (ImGui.ArrowButton("InspectorLeft", ImGuiDir.Left))
                    {
                        --this.selected;
                    }

                    ImGui.SameLine();
                    ImGui.Text($"{this.selected + 1}/{textures.Count}");

                    ImGui.SameLine();
                    if (ImGui.ArrowButton("InspectorRight", ImGuiDir.Right))
                    {
                        ++this.selected;
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Remove"))
                    {
                        this.PropertyInspector.Textures.Remove(texture);
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Edit"))
                    {
                        var temp = Path.Join(Path.GetTempPath(), $"{DateTime.Now.Ticks}.png");
                        try
                        {
                            using (var stream = File.Create(temp))
                            {
                                texture.SaveAsPng(stream, texture.Width, texture.Height);
                            }
                            Process.Start(@"C:\Program Files\paint.net\PaintDotNet.exe", temp);
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Warning(ex, "Could not save or open file {@file}", temp);
                        }
                    }
                }

                ImGui.End();
            }
        }
    }
}
