using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class MaterialEditor : AEditor<Material>
    {
        private readonly ImGuiRenderer GuiRenderer;
        private Vector3 colorPicker;
        private IntPtr popup;

        public MaterialEditor(ImGuiRenderer guiRenderer)
        {
            this.GuiRenderer = guiRenderer;
        }

        public override bool Draw(string name, Func<Material> get, Action<Material> set)
        {
            var material = get();

            var albedo = this.DrawTexture(nameof(Material.Albedo), material.Albedo);
            var normal = this.DrawTexture(nameof(Material.Normal), material.Normal);
            var metalicness = this.DrawTexture(nameof(Material.Metalicness), material.Metalicness);
            var roughness = this.DrawTexture(nameof(Material.Roughness), material.Roughness);
            var ambientOcclusion = this.DrawTexture(nameof(Material.AmbientOcclusion), material.AmbientOcclusion);

            if (metalicness != material.Metalicness ||
                roughness != material.Roughness ||
                ambientOcclusion != material.AmbientOcclusion ||
                albedo != material.Albedo ||
                normal != material.Normal)
            {
                material.Metalicness = metalicness;
                material.Roughness = roughness;
                material.AmbientOcclusion = ambientOcclusion;
                material.Albedo = albedo;
                material.Normal = normal;

                return true;
            }

            return false;
        }

        private Texture2D DrawTexture(string name, Texture2D texture)
        {
            if (texture.Tag == null)
            {
                texture.Tag = this.GuiRenderer.BindTexture(texture);
            }

            ImGui.Text($"{name} : {texture.Format} ({texture.Width}x{texture.Height}x{texture.LevelCount})");

            var maxWidth = Math.Min(ImGui.CalcItemWidth(), 1024);
            var size = ImageUtilities.FitToBounds(texture.Width, texture.Height, maxWidth, 1024);

            if (ImGui.ImageButton((IntPtr)texture.Tag, size, Vector2.Zero, Vector2.One, 1))
            {
                this.popup = (IntPtr)texture.Tag;
            }

            if (this.popup == (IntPtr)texture.Tag)
            {
                texture = this.PickTexture(texture);
            }

            return texture;
        }

        private Texture2D PickTexture(Texture2D texture)
        {
            ImGui.OpenPopup($"Color Picker");
            if (ImGui.BeginPopup($"Color Picker"))
            {
                ImGui.ColorPicker3("Color", ref this.colorPicker);
                if (ImGui.Button("Apply"))
                {
                    texture = new Texture2D(texture.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                    texture.SetData(new Color[] { new Color(this.colorPicker) });

                    ImGui.CloseCurrentPopup();
                    this.popup = IntPtr.Zero;
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                    this.popup = IntPtr.Zero;
                }
                ImGui.EndPopup();
            }

            return texture;
        }

        private static float DrawSlider(string name, float value)
        {
            ImGui.SliderFloat(name, ref value, 0.0f, 1.0f);
            return value;
        }
    }
}
