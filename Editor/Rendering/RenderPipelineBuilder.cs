using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline;
using MiniEngine.Pipeline.Basics.Systems;
using MiniEngine.Pipeline.Debug.Extensions;
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
using MiniEngine.Systems;
using MiniEngine.Telemetry;

namespace MiniEngine.Rendering
{
    public sealed class RenderPipelineBuilder
    {
        private readonly EffectFactory EffectFactory;
        private readonly Resolver<ISystem> Systems;

        public RenderPipelineBuilder(EffectFactory effectFactory, Resolver<ISystem> systems)
        {
            this.EffectFactory = effectFactory;
            this.Systems = systems;
        }

        public RenderPipeline Build(GraphicsDevice device, IMeterRegistry meterRegistry, RenderPipelineSettings settings)
        {
            var pipeline = RenderPipeline.Create(device, meterRegistry)
                .ClearRenderTargetSet()
                .UpdateSystem(this.Systems.Get<OffsetSystem>());

            this.AddDynamicTextures(pipeline);
            this.AddShadows(pipeline, device, meterRegistry, settings);
            this.AddModels(pipeline, device, meterRegistry, settings);
            this.AddParticles(pipeline, device, meterRegistry, settings);
            this.AddDebug(pipeline, settings);

            return pipeline;
        }

        private void AddDynamicTextures(RenderPipeline pipeline)
            => pipeline.UpdateSystem(this.Systems.Get<DynamicTextureSystem>());


        private void AddShadows(RenderPipeline pipeline, GraphicsDevice device, IMeterRegistry meterRegistry, RenderPipelineSettings settings)
        {
            if (settings.EnableShadows)
            {
                var shadowPipeline = ShadowPipeline.Create(device, meterRegistry)
                    .RenderShadowMaps(this.Systems.Get<ShadowMapSystem>());

                pipeline
                    .UpdateSystem(this.Systems.Get<CascadedShadowMapSystem>())
                    .RenderShadows(shadowPipeline);
            }
        }

        private void AddModels(RenderPipeline pipeline, GraphicsDevice device, IMeterRegistry meterRegistry, RenderPipelineSettings settings)
        {
            if (settings.EnableModels)
            {
                pipeline.UpdateSystem(this.Systems.Get<BoundsSystem>());

                var modelPipeline = ModelPipeline.Create(device, meterRegistry)
                    .ClearModelRenderTargets()
                    .RenderGeometry(this.Systems.Get<GeometrySystem>())
                    .RenderModelBatch();

                if (settings.EnableProjectors)
                {
                    var projectorPipeline = ProjectorPipeline.Create(device, meterRegistry);
                    var projectorSystem = this.Systems.Get<ProjectorSystem>();
                    projectorSystem.Technique = settings.ProjectorTechnique;
                    projectorPipeline.RenderProjectors(projectorSystem);

                    modelPipeline.RenderProjectors(projectorPipeline);
                }

                if (EnableLights(settings))
                {
                    var ls = settings.LightSettings;
                    var lightingPipeline = LightingPipeline.Create(device, meterRegistry)
                        .ClearLightTargets()
                        .EnableIf(ls.EnableAmbientLights, x => x.RenderAmbientLight(this.Systems.Get<AmbientLightSystem>(), ls.EnableSSAO))
                        .EnableIf(ls.EnableDirectionalLights, x => x.RenderDirectionalLights(this.Systems.Get<DirectionalLightSystem>()))
                        .EnableIf(ls.EnablePointLights, x => x.RenderPointLights(this.Systems.Get<PointLightSystem>()))
                        .EnableIf(ls.EnableShadowCastingLights, x => x.RenderShadowCastingLights(this.Systems.Get<ShadowCastingLightSystem>()))
                        .EnableIf(ls.EnableSunLights, x => x.RenderSunlights(this.Systems.Get<SunlightSystem>()));

                    modelPipeline.RenderLights(lightingPipeline);
                }

                var combineEffect = this.EffectFactory.Construct<CombineEffect>();
                var fxaaEffect = this.EffectFactory.Construct<FxaaEffect>();
                modelPipeline
                    .CombineDiffuseWithLighting(combineEffect)
                    .AntiAlias(fxaaEffect, settings.ModelSettings.FxaaFactor);

                pipeline.RenderModels(this.Systems.Get<ModelSystem>(), modelPipeline);
            }
        }

        private static bool EnableLights(RenderPipelineSettings settings)
        {
            var ls = settings.LightSettings;
            return ls.EnableAmbientLights || ls.EnableDirectionalLights || ls.EnablePointLights || ls.EnableShadowCastingLights || ls.EnableSunLights;
        }

        private void AddParticles(RenderPipeline pipeline, GraphicsDevice device, IMeterRegistry meterRegistry, RenderPipelineSettings settings)
        {
            if (settings.EnableParticles)
            {
                var particlePipeline = ParticlePipeline.Create(device, meterRegistry).ClearParticleRenderTargets()
                    .RenderTransparentParticles(this.Systems.Get<AveragedParticleSystem>())
                    .RenderAdditiveParticles(this.Systems.Get<AdditiveParticleSystem>());

                pipeline.RenderParticles(particlePipeline);
            }
        }

        private void AddDebug(RenderPipeline pipeline, RenderPipelineSettings settings)
        {
            pipeline
                .EnableIf(settings.EnableDebugLines, x => x.RenderDebugLines(this.Systems.Get<LineSystem>()))
                .EnableIf(settings.Enable3DOutlines, x => x.Render3DOutline(this.Systems.Get<BoundarySystem>()))
                .EnableIf(settings.Enable2DOutlines, x => x.Render2DOutline(this.Systems.Get<BoundarySystem>()))
                .EnableIf(settings.EnableIcons, x => x.RenderIcons(this.Systems.Get<IconSystem>()));
        }
    }
}
