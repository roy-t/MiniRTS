using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline;
using MiniEngine.Pipeline.Debug.Systems;
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
using MiniEngine.Telemetry;

namespace MiniEngine.Rendering
{
    public sealed class PipelineBuilder
    {
        private readonly GraphicsDevice Device;
        private readonly ShadowMapSystem ShadowMapSystem;
        private readonly ModelSystem ModelSystem;
        private readonly AveragedParticleSystem TransparentParticleSystem;
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
        private readonly BoundarySystem BoundarySystem;
        private readonly IconSystem IconSystem;
        private readonly IMeterRegistry MeterRegistry;


        public PipelineBuilder(GraphicsDevice device, ShadowMapSystem shadowMapSystem, ModelSystem modelSystem, AveragedParticleSystem transparentParticleSystem, AdditiveParticleSystem additiveParticleSystem, ProjectorSystem projectorSystem, EffectFactory effectFactory, AmbientLightSystem ambientLightSystem, DirectionalLightSystem directionalLightSystem, PointLightSystem pointLightSystem, CascadedShadowMapSystem cascadedShadowMapSystem, ShadowCastingLightSystem shadowCastingLightSystem, SunlightSystem sunlightSystem, BoundarySystem boundarySystem, IconSystem iconSystem)
        {
            this.Device = device;
            this.ShadowMapSystem           = shadowMapSystem;
            this.ModelSystem               = modelSystem;
            this.TransparentParticleSystem = transparentParticleSystem;
            this.AdditiveParticleSystem    = additiveParticleSystem;
            this.ProjectorSystem           = projectorSystem;
            this.CombineEffect             = effectFactory.Construct<CombineEffect>();
            this.FxaaEffect                = effectFactory.Construct<FxaaEffect>();
            this.AmbientLightSystem        = ambientLightSystem;
            this.DirectionalLightSystem    = directionalLightSystem;
            this.PointLightSystem          = pointLightSystem;
            this.CascadedShadowMapSystem   = cascadedShadowMapSystem;
            this.ShadowCastingLightSystem  = shadowCastingLightSystem;
            this.SunlightSystem            = sunlightSystem;
            this.BoundarySystem            = boundarySystem;
            this.IconSystem                = iconSystem;

            this.MeterRegistry = new NullMeterRegistry();
        }

        public void AddParticlePipeline(RenderPipeline pipeline)
        {
            var particlePipeline = ParticlePipeline.Create(this.Device, this.MeterRegistry);
                     
            particlePipeline
                .ClearParticleRenderTargets()
                .RenderTransparentParticles(this.TransparentParticleSystem)
                .RenderAdditiveParticles(this.AdditiveParticleSystem);

            pipeline
                .ClearRenderTargetSet()
                .RenderParticles(particlePipeline);
        }

        public void AddAll(RenderPipeline pipeline)
        {
            var shadowPipeline    = ShadowPipeline.Create(this.Device, this.MeterRegistry);
            var lightingPipeline  = LightingPipeline.Create(this.Device, this.MeterRegistry);
            var modelPipeline     = ModelPipeline.Create(this.Device, this.MeterRegistry);
            var particlePipeline  = ParticlePipeline.Create(this.Device, this.MeterRegistry);
            var projectorPipeline = ProjectorPipeline.Create(this.Device, this.MeterRegistry);
           

            shadowPipeline
                .RenderShadowMaps(this.ShadowMapSystem);

            lightingPipeline
                .ClearLightTargets()
                .RenderAmbientLight(this.AmbientLightSystem, true)
                .RenderDirectionalLights(this.DirectionalLightSystem)
                .RenderPointLights(this.PointLightSystem)
                .RenderShadowCastingLights(this.ShadowCastingLightSystem)
                .RenderSunlights(this.SunlightSystem);

            projectorPipeline
                    .RenderProjectors(this.ProjectorSystem);
            this.ProjectorSystem.Technique = Effects.Techniques.ProjectorEffectTechniques.Projector;

            modelPipeline
                .ClearModelRenderTargets()
                .RenderModelBatch()
                .RenderProjectors(projectorPipeline)
                .RenderLights(lightingPipeline)
                .CombineDiffuseWithLighting(this.CombineEffect)
                .AntiAlias(this.FxaaEffect, 4);

            particlePipeline
                .ClearParticleRenderTargets()
                .RenderTransparentParticles(this.TransparentParticleSystem)
                .RenderAdditiveParticles(this.AdditiveParticleSystem);

            pipeline
                .ClearRenderTargetSet()
                .RenderShadows(shadowPipeline)
                .RenderModels(this.ModelSystem, modelPipeline)
                .RenderParticles(particlePipeline);                
        }
    }  
}
