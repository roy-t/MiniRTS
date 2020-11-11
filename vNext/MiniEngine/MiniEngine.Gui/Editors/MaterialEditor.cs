using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Geometry;

namespace MiniEngine.Gui.Editors
{
    public sealed class MaterialEditor : AEditor<Material>
    {
        private readonly ImGuiRenderer GuiRenderer;
        private Vector3 colorPicker;
        private bool popup;

        public MaterialEditor(ImGuiRenderer guiRenderer)
        {
            this.GuiRenderer = guiRenderer;
        }

        public override void Draw(string name, Func<Material> get, Action<Material> set)
        {
            var material = get();

            material.Metalicness = DrawSlider(nameof(material.Metalicness), material.Metalicness);
            material.Roughness = DrawSlider(nameof(material.Roughness), material.Roughness);
            material.AmbientOcclusion = DrawSlider(nameof(material.AmbientOcclusion), material.AmbientOcclusion);

            material.Diffuse = this.DrawTexture(nameof(Material.Diffuse), material.Diffuse);
            material.Normal = this.DrawTexture(nameof(Material.Normal), material.Normal);
        }

        private Texture2D DrawTexture(string name, Texture2D texture)
        {
            if (texture.Tag == null)
            {
                texture.Tag = this.GuiRenderer.BindTexture(texture);
            }

            ImGui.Text($"{name} : {texture.Format} ({texture.Width}x{texture.Height}x{texture.LevelCount})");
            ImGui.Image((IntPtr)texture.Tag, new Vector2(texture.Width, texture.Height));

            // TODO: color picker popup?
            //this.popup |= ImGui.Button("Change");
            //if (this.popup)
            //{
            //    ImGui.OpenPopup($"C{name}");
            //}
            //if (ImGui.BeginPopup($"C{name}"))
            //{
            //    ImGui.ColorPicker3("Color", ref this.colorPicker);
            //    if (ImGui.Button("Apply"))
            //    {
            //        texture = new Texture2D(texture.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            //        texture.SetData(new Color[] { new Color(this.colorPicker) });
            //    }
            //    ImGui.EndPopup();
            //}

            return texture;
        }

        private static float DrawSlider(string name, float value)
        {
            ImGui.SliderFloat(name, ref value, 0.0f, 1.0f);
            return value;
        }
    }
}
