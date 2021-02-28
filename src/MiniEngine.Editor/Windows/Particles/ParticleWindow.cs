using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Packs;
using MiniEngine.Graphics.Particles;
using MiniEngine.Gui;
using MiniEngine.Gui.Windows;
using MiniEngine.SceneManagement;
using MiniEngine.Systems.Components;

namespace MiniEngine.Editor.Windows.Particles
{
    [Service]
    public sealed class ParticleWindow : IWindow
    {
        private readonly WindowService WindowService;
        private readonly ImGuiRenderer ImGuiRenderer;
        private readonly TexturePicker TexturePicker;
        private readonly ComponentAdministrator ComponentAdministrator;
        private readonly TexturePack TexturePack;

        public ParticleWindow(WindowService windowService, ImGuiRenderer imGuiRenderer, TexturePicker texturePicker, ContentStack content, ComponentAdministrator componentAdministrator)
        {
            this.WindowService = windowService;
            this.ImGuiRenderer = imGuiRenderer;
            this.TexturePicker = texturePicker;
            this.ComponentAdministrator = componentAdministrator;
            this.TexturePack = content.Load<TexturePack>("Particles/particles");
        }

        private ParticleEmitter? emitter;

        public string Name => "Particles";

        public bool AllowTransparency => false;

        public void SetEmitter(ParticleEmitter emitter)
        {
            this.emitter = emitter;
            this.WindowService.OpenWindow(this);
        }


        public void RenderContents()
        {
            var emitters = this.GetAllEmitters();
            if (emitters.Count == 0)
            {
                return;
            }

            if (this.emitter == null)
            {
                this.emitter = emitters[0];
            }

            var index = 0;
            if (this.emitter != null)
            {
                index = emitters.IndexOf(this.emitter);
            }

            if (ImGui.Combo("Emitters", ref index, emitters.Select(e => e.Texture.Name).ToArray(), emitters.Count))
            {
                this.emitter = emitters[index];
            }

            var texture = this.emitter!.Texture;
            this.ImGuiRenderer.BindTexture(texture);
            var bounds = ImageUtilities.FitToBounds(texture.Width, texture.Height, 64, 64);
            if (ImGui.ImageButton((IntPtr)texture.Tag, bounds))
            {
                this.TexturePicker.Open();
            }

            this.emitter.Texture = this.TexturePicker.Pick(texture, this.TexturePack);
        }


        private List<ParticleEmitter> GetAllEmitters()
            => this.ComponentAdministrator.GetComponents<ParticleFountainComponent>().SelectMany(f => f.Emitters).ToList();
    }
}
