using ImGuiNET;
using MiniEngine.Rendering;

namespace MiniEngine.UI
{
    public sealed class RenderingMenu
    {
        private readonly DeferredRenderPipeline RenderPipeline;

        public RenderingMenu(UIState ui, DeferredRenderPipeline renderPipeline)
        {
            this.RenderPipeline = renderPipeline;
        }

        public void Render()
        {
            if (ImGui.BeginMenu("Rendering"))
            {
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.EnableModels), this.RenderPipeline.Settings.EnableModels, false, true, x => { this.RenderPipeline.Settings.EnableModels = (bool)x; this.RenderPipeline.Recreate(); });
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.EnableParticles), this.RenderPipeline.Settings.EnableParticles, false, true, x => { this.RenderPipeline.Settings.EnableParticles = (bool)x; this.RenderPipeline.Recreate(); });
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.EnableShadows), this.RenderPipeline.Settings.EnableShadows, false, true, x => { this.RenderPipeline.Settings.EnableShadows = (bool)x; this.RenderPipeline.Recreate(); });
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.Enable2DOutlines), this.RenderPipeline.Settings.Enable2DOutlines, false, true, x => { this.RenderPipeline.Settings.Enable2DOutlines = (bool)x; this.RenderPipeline.Recreate(); });
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.Enable3DOutlines), this.RenderPipeline.Settings.Enable3DOutlines, false, true, x => { this.RenderPipeline.Settings.Enable3DOutlines = (bool)x; this.RenderPipeline.Recreate(); });
                ImGui.Separator();
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.ModelSettings.FxaaFactor), this.RenderPipeline.Settings.ModelSettings.FxaaFactor, 0, 16, x => { this.RenderPipeline.Settings.ModelSettings.FxaaFactor = (int)x; this.RenderPipeline.Recreate(); });
                ImGui.Separator();
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.LightSettings.EnableAmbientLights), this.RenderPipeline.Settings.LightSettings.EnableAmbientLights, false, true, x => { this.RenderPipeline.Settings.LightSettings.EnableAmbientLights = (bool)x; this.RenderPipeline.Recreate(); });
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.LightSettings.EnableDirectionalLights), this.RenderPipeline.Settings.LightSettings.EnableDirectionalLights, false, true, x => { this.RenderPipeline.Settings.LightSettings.EnableDirectionalLights = (bool)x; this.RenderPipeline.Recreate(); });
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.LightSettings.EnablePointLights), this.RenderPipeline.Settings.LightSettings.EnablePointLights, false, true, x => { this.RenderPipeline.Settings.LightSettings.EnablePointLights = (bool)x; this.RenderPipeline.Recreate(); });
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.LightSettings.EnableShadowCastingLights), this.RenderPipeline.Settings.LightSettings.EnableShadowCastingLights, false, true, x => { this.RenderPipeline.Settings.LightSettings.EnableShadowCastingLights = (bool)x; this.RenderPipeline.Recreate(); });
                Editors.CreateEditor(nameof(this.RenderPipeline.Settings.LightSettings.EnableSunLights), this.RenderPipeline.Settings.LightSettings.EnableSunLights, false, true, x => { this.RenderPipeline.Settings.LightSettings.EnableSunLights = (bool)x; this.RenderPipeline.Recreate(); });

                ImGui.EndMenu();
            }
        }
    }
}
