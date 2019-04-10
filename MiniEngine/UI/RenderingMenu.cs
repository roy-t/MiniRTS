using ImGuiNET;
using MiniEngine.Rendering;
using MiniEngine.Systems.Components;

namespace MiniEngine.UI
{
    public sealed class RenderingMenu
    {
        private readonly DeferredRenderPipeline RenderPipeline;
        private readonly Editors Editors;

        public RenderingMenu(Editors editors, UIState ui, DeferredRenderPipeline renderPipeline)
        {
            this.Editors = editors;
            this.RenderPipeline = renderPipeline;
        }

        public void Render()
        {
            // TODO: enabling features in the deferred render pipeline is overly simplistic now, maybe style it as a true options menu
            // that serializes/deserializes settings from a file, and make it so that the a class can cosntruct the deferred renderpipeline from that file
            if (ImGui.BeginMenu("Rendering"))
            {
                this.Editors.Create(nameof(this.RenderPipeline.Settings.EnableModels), this.RenderPipeline.Settings.EnableModels, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.EnableModels = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.EnableProjectors), this.RenderPipeline.Settings.EnableProjectors, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.EnableProjectors = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.EnableParticles), this.RenderPipeline.Settings.EnableParticles, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.EnableParticles = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.EnableShadows), this.RenderPipeline.Settings.EnableShadows, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.EnableShadows = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.Enable2DOutlines), this.RenderPipeline.Settings.Enable2DOutlines, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.Enable2DOutlines = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.Enable3DOutlines), this.RenderPipeline.Settings.Enable3DOutlines, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.Enable3DOutlines = (bool)x; this.RenderPipeline.Recreate(); });
                ImGui.Separator();
                this.Editors.Create(nameof(this.RenderPipeline.Settings.ModelSettings.FxaaFactor), this.RenderPipeline.Settings.ModelSettings.FxaaFactor, new MinMaxDescription(0, 16), x => { this.RenderPipeline.Settings.ModelSettings.FxaaFactor = (int)x; this.RenderPipeline.Recreate(); });
                ImGui.Separator();
                this.Editors.Create(nameof(this.RenderPipeline.Settings.LightSettings.EnableAmbientLights), this.RenderPipeline.Settings.LightSettings.EnableAmbientLights, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.LightSettings.EnableAmbientLights = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.LightSettings.EnableDirectionalLights), this.RenderPipeline.Settings.LightSettings.EnableDirectionalLights, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.LightSettings.EnableDirectionalLights = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.LightSettings.EnablePointLights), this.RenderPipeline.Settings.LightSettings.EnablePointLights, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.LightSettings.EnablePointLights = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.LightSettings.EnableShadowCastingLights), this.RenderPipeline.Settings.LightSettings.EnableShadowCastingLights, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.LightSettings.EnableShadowCastingLights = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.LightSettings.EnableSunLights), this.RenderPipeline.Settings.LightSettings.EnableSunLights, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.LightSettings.EnableSunLights = (bool)x; this.RenderPipeline.Recreate(); });
                this.Editors.Create(nameof(this.RenderPipeline.Settings.LightSettings.EnableSSAO), this.RenderPipeline.Settings.LightSettings.EnableSSAO, MinMaxDescription.ZeroToOne, x => { this.RenderPipeline.Settings.LightSettings.EnableSSAO = (bool)x; this.RenderPipeline.Recreate(); });

                ImGui.EndMenu();
            }
        }
    }
}
