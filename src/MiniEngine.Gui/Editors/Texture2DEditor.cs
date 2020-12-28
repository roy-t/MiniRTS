using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class Texture2DEditor : AEditor<Texture2D>
    {
        private readonly ImGuiRenderer GuiRenderer;
        private readonly PropertyInspector PropertyInspector;
        private Vector3 colorPicker;
        private IntPtr popup;

        public Texture2DEditor(ImGuiRenderer guiRenderer, PropertyInspector propertyInspector)
        {
            this.GuiRenderer = guiRenderer;
            this.PropertyInspector = propertyInspector;
        }

        public override bool Draw(string name, Func<Texture2D> get, Action<Texture2D> set)
        {
            var texture = get();
            var newTexture = this.DrawTextureButton(name, texture);

            if (texture != newTexture)
            {
                set(newTexture);
                return true;
            }

            return false;
        }

        private Texture2D DrawTextureButton(string name, Texture2D texture)
        {
            if (texture.Tag == null)
            {
                texture.Tag = this.GuiRenderer.BindTexture(texture);
            }

            ImGui.Text($"{name} : {texture.Format} ({texture.Width}x{texture.Height}x{texture.LevelCount})");

            var maxWidth = Math.Min(ImGui.CalcItemWidth(), 128);
            var size = ImageUtilities.FitToBounds(texture.Width, texture.Height, maxWidth, 1024);

            if (ImGui.ImageButton((IntPtr)texture.Tag, size, Vector2.Zero, Vector2.One, 1))
            {
                this.popup = (IntPtr)texture.Tag;
            }

            if (ImGui.Button($"inspect {name}"))
            {
                this.PropertyInspector.Textures.Add(texture);
            }

            if (this.popup == (IntPtr)texture.Tag)
            {
                texture = this.PickTexture(texture);
            }

            return texture;
        }

        private Texture2D PickTexture(Texture2D texture)
        {
            ImGui.OpenPopup($"Texture Viewer");
            if (ImGui.BeginPopup($"Texture Viewer"))
            {
                DrawLargePreview(texture);
                ImGui.Separator();
                this.DrawReplacementPicker();
                texture = this.ConfirmReplacement(texture);

                ImGui.EndPopup();
            }

            return texture;
        }

        private Texture2D ConfirmReplacement(Texture2D texture)
        {
            if (ImGui.Button("Replace"))
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

            return texture;
        }

        private void DrawReplacementPicker()
        {
            ImGui.Text("Replacement");
            ImGui.ColorPicker3("Color", ref this.colorPicker);
        }

        private static void DrawLargePreview(Texture2D texture)
        {
            ImGui.Text("Texture");
            var size = ImageUtilities.FitToBounds(texture.Width, texture.Height, 1024, 650);
            ImGui.Image((IntPtr)texture.Tag, size, Vector2.Zero, Vector2.One);
        }
    }
}
