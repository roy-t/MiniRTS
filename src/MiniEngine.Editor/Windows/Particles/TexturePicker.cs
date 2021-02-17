using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Packs;
using MiniEngine.Gui;

namespace MiniEngine.Editor.Windows.Particles
{
    [Service]
    public sealed class TexturePicker
    {
        private const float ButtonWidth = 64.0f;
        private const float ButtonHeight = 64.0f;

        private readonly ImGuiRenderer ImGuiRenderer;

        public TexturePicker(ImGuiRenderer imGuiRenderer)
        {
            this.ImGuiRenderer = imGuiRenderer;
        }

        public void Open()
            => ImGui.OpenPopup("Texture Picker");

        public Texture2D Pick(Texture2D current, TexturePack pack)
        {
            var textures = pack.Textures;

            if (ImGui.BeginPopupModal("Texture Picker"))
            {
                this.BindTextures(textures);

                ImGui.Text("Current:");
                if (ImageButton(current))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.Text($"Pack: {pack.Name}");

                var style = ImGui.GetStyle();
                var windowMaxX = ImGui.GetWindowPos().X + ImGui.GetWindowContentRegionMax().X;

                for (var i = 0; i < textures.Count; i++)
                {
                    ImGui.PushID(i);
                    var texture = textures[i];
                    if (ImageButton(texture))
                    {
                        current = texture;
                        ImGui.CloseCurrentPopup();
                    }
                    var lastButtonX = ImGui.GetItemRectMax().X;
                    var nextButtonX = lastButtonX + style.ItemSpacing.X + ButtonWidth;
                    if (i + 1 < textures.Count && nextButtonX < windowMaxX)
                    {
                        ImGui.SameLine();
                    }
                    ImGui.PopID();
                }


                ImGui.EndPopup();
            }

            return current;
        }

        private void BindTextures(IReadOnlyList<Texture2D> textures)
        {
            for (var i = 0; i < textures.Count; i++)
            {
                this.ImGuiRenderer.BindTexture(textures[i]);
            }
        }

        private static bool ImageButton(Texture2D texture)
        {
            var size = ImageUtilities.FitToBounds(texture.Width, texture.Height, ButtonWidth, ButtonHeight);
            return ImGui.ImageButton((IntPtr)texture.Tag, size);
        }
    }
}
