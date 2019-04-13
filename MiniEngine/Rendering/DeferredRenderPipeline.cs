using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Pipeline;
using MiniEngine.Pipeline.Extensions;
using MiniEngine.Pipeline.Lights;
using MiniEngine.Pipeline.Lights.Extensions;
using MiniEngine.Pipeline.Lights.Systems;
using MiniEngine.Pipeline.Models;
using MiniEngine.Pipeline.Models.Extensions;
using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Pipeline.Particles;
using MiniEngine.Pipeline.Particles.Extensions;
using MiniEngine.Pipeline.Particles.Systems;
using MiniEngine.Pipeline.Projectors;
using MiniEngine.Pipeline.Projectors.Extensions;
using MiniEngine.Pipeline.Projectors.Systems;
using MiniEngine.Pipeline.Shadows;
using MiniEngine.Pipeline.Shadows.Extensions;
using MiniEngine.Pipeline.Shadows.Systems;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Telemetry;
using MiniEngine.Units;

namespace MiniEngine.Rendering
{
    public sealed class DeferredRenderPipeline
    {
        private readonly GBuffer GBuffer;
        private readonly RenderPipelineInput Input;
        
        private readonly ShadowMapSystem ShadowMapSystem;
        private readonly ModelSystem ModelSystem;
        private readonly TransparentParticleSystem TransparentParticleSystem;
        private readonly AdditiveParticleSystem AdditiveParticleSystem;
        private readonly ProjectorSystem ProjectorSystem;
        private readonly CombineEffect CombineEffect;
        private readonly FxaaEffect FxaaEffect;
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly DirectionalLightSystem DirectionalLightSystem;
        private readonly PointLightSystem PointLightSystem;
        private readonly CascadedShadowMapSystem CascadedShadowMapSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;
        private readonly SunlightSystem SunlightSystem;
        private readonly OutlineSystem OutlineSystem;        

        private readonly ShadowPipeline ShadowPipeline;
        private readonly LightingPipeline LightingPipeline;
        private readonly ModelPipeline ModelPipeline;
        private readonly ParticlePipeline ParticlePipeline;
        private readonly ProjectorPipeline ProjectorPipeline;

        private readonly RenderPipeline Pipeline;
        private readonly Pass rootPass;

        public DeferredRenderPipeline(
            GraphicsDevice device,
            ShadowMapSystem shadowMapSystem,
            ModelSystem modelSystem,
            TransparentParticleSystem particleSystem,
            AdditiveParticleSystem additiveParticleSystem,
            ProjectorSystem projectorSystem,
            CombineEffect combineEffect,
            FxaaEffect fxaaEffect,
            AmbientLightSystem ambientLightSystem,
            DirectionalLightSystem directionalLightSystem,
            PointLightSystem pointLightSystem,
            CascadedShadowMapSystem cascadedShadowMapSystem,
            ShadowCastingLightSystem shadowCastingLightSystem,
            SunlightSystem sunlightSystem,
            OutlineSystem outlineSystem,
            IMeterRegistry meterRegistry)
        {
            this.ShadowMapSystem = shadowMapSystem;
            this.ModelSystem = modelSystem;
            this.TransparentParticleSystem = particleSystem;
            this.AdditiveParticleSystem = additiveParticleSystem;
            this.ProjectorSystem = projectorSystem;
            this.CombineEffect = combineEffect;
            this.FxaaEffect = fxaaEffect;
            this.AmbientLightSystem = ambientLightSystem;
            this.DirectionalLightSystem = directionalLightSystem;
            this.PointLightSystem = pointLightSystem;
            this.CascadedShadowMapSystem = cascadedShadowMapSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
            this.SunlightSystem = sunlightSystem;
            this.OutlineSystem = outlineSystem;

            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;
            this.GBuffer = new GBuffer(device, width, height);

            this.Input = new RenderPipelineInput();

            this.Settings = new RenderPipelineSettings();

            this.ShadowPipeline = ShadowPipeline.Create(device, meterRegistry);
            this.LightingPipeline = LightingPipeline.Create(device, meterRegistry);
            this.ModelPipeline = ModelPipeline.Create(device, meterRegistry);
            this.ParticlePipeline = ParticlePipeline.Create(device, meterRegistry);
            this.ProjectorPipeline = ProjectorPipeline.Create(device, meterRegistry);

            this.Pipeline = RenderPipeline.Create(device, meterRegistry);
            this.rootPass = new Pass(PassType.Opaque, 0);

            this.Recreate();
        }

        public RenderPipelineSettings Settings { get; }

        public void Recreate()
        {
            this.ShadowPipeline.Clear();
            this.LightingPipeline.Clear();
            this.ModelPipeline.Clear();
            this.ParticlePipeline.Clear();
            this.ProjectorPipeline.Clear();

            this.Pipeline.Clear();

            this.ShadowPipeline
                .RenderShadowMaps(this.ShadowMapSystem);
            
            var ls = this.Settings.LightSettings;
            this.LightingPipeline
                .ClearLightTargets()
                .EnableIf(ls.EnableAmbientLights, x => x.RenderAmbientLight(this.AmbientLightSystem, ls.EnableSSAO))
                .EnableIf(ls.EnableDirectionalLights, x => x.RenderDirectionalLights(this.DirectionalLightSystem))
                .EnableIf(ls.EnablePointLights, x => x.RenderPointLights(this.PointLightSystem))
                .EnableIf(ls.EnableShadowCastingLights, x => x.RenderShadowCastingLights(this.ShadowCastingLightSystem))
                .EnableIf(ls.EnableSunLights, x => x.RenderSunlights(this.SunlightSystem));

            if (this.Settings.EnableProjectors)
            {
                this.ProjectorPipeline             
                    .RenderProjectors(this.ProjectorSystem);

                this.ProjectorSystem.Technique = this.Settings.ProjectorTechnique;
            }

            this.ModelPipeline
                .ClearModelRenderTargets()
                .RenderModelBatch()
                .RenderProjectors(this.ProjectorPipeline)
                .RenderLights(this.LightingPipeline)
                .CombineDiffuseWithLighting(this.CombineEffect)
                .AntiAlias(this.FxaaEffect, this.Settings.ModelSettings.FxaaFactor);            

            this.ParticlePipeline
                .ClearParticleRenderTargets()
                .RenderTransparentParticles(this.TransparentParticleSystem)
                .RenderAdditiveParticles(this.AdditiveParticleSystem);            

            this.Pipeline
                .ClearRenderTargetSet()
                .UpdateSystem(this.CascadedShadowMapSystem)
                .UpdateSystem(this.TransparentParticleSystem)
                .UpdateSystem(this.AdditiveParticleSystem)
                .EnableIf(this.Settings.EnableShadows, x => x.RenderShadows(this.ShadowPipeline))
                .EnableIf(this.Settings.EnableModels, x => x.RenderModels(this.ModelSystem, this.ModelPipeline))
                .EnableIf(this.Settings.EnableParticles, x => x.RenderParticles(this.ParticlePipeline))                
                .EnableIf(this.Settings.Enable3DOutlines, x => x.Render3DOutline(this.OutlineSystem))
                .EnableIf(this.Settings.Enable2DOutlines, x => x.Render2DOutline(this.OutlineSystem));
        }
      
        public RenderTarget2D Render(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Input.Update(camera, elapsed, this.GBuffer, this.rootPass);
            this.Pipeline.Execute(this.Input, "root");
            return this.GBuffer.FinalTarget;
        }

        public GBuffer GetGBuffer() => this.GBuffer;
    }
}