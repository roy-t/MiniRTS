using System.Collections.Generic;
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
    public sealed class RenderPipelineBuilder : APipelineBuilder
    {
        private readonly EffectFactory EffectFactory;

        public RenderPipelineBuilder(EffectFactory effectFactory, IEnumerable<ISystem> systems)
            : base(systems)
        {
            this.EffectFactory = effectFactory;
        }

        public RenderPipeline Build(GraphicsDevice device, IMeterRegistry meterRegistry, RenderPipelineSettings settings)
        {
            var pipeline = RenderPipeline.Create(device, meterRegistry)
                .ClearRenderTargetSet()
                .UpdateSystem(this.GetSystem<OffsetSystem>());

            this.AddDynamicTextures(pipeline);
            this.AddShadows(pipeline, device, meterRegistry, settings);
            this.AddModels(pipeline, device, meterRegistry, settings);
            this.AddParticles(pipeline, device, meterRegistry, settings);
            this.AddDebug(pipeline, settings);

            return pipeline;
        }

        private void AddDynamicTextures(RenderPipeline pipeline)
            => pipeline.UpdateSystem(this.GetSystem<DynamicTextureSystem>());


        private void AddShadows(RenderPipeline pipeline, GraphicsDevice device, IMeterRegistry meterRegistry, RenderPipelineSettings settings)
        {
            if (settings.EnableShadows)
            {
                var shadowPipeline = ShadowPipeline.Create(device, meterRegistry)
                    .RenderShadowMaps(this.GetSystem<ShadowMapSystem>());

                pipeline
                    .UpdateSystem(this.GetSystem<CascadedShadowMapSystem>())
                    .RenderShadows(shadowPipeline);
            }
        }

        private void AddModels(RenderPipeline pipeline, GraphicsDevice device, IMeterRegistry meterRegistry, RenderPipelineSettings settings)
        {
            if (settings.EnableModels)
            {
                pipeline.UpdateSystem(this.GetSystem<BoundsSystem>());

                var modelPipeline = ModelPipeline.Create(device, meterRegistry)
                    .ClearModelRenderTargets()
                    .RenderModelBatch();

                if (settings.EnableProjectors)
                {
                    var projectorPipeline = ProjectorPipeline.Create(device, meterRegistry);
                    var projectorSystem = this.GetSystem<ProjectorSystem>();
                    projectorSystem.Technique = settings.ProjectorTechnique;
                    projectorPipeline.RenderProjectors(projectorSystem);

                    modelPipeline.RenderProjectors(projectorPipeline);
                }

                if (EnableLights(settings))
                {
                    var ls = settings.LightSettings;
                    var lightingPipeline = LightingPipeline.Create(device, meterRegistry)
                        .ClearLightTargets()
                        .EnableIf(ls.EnableAmbientLights, x => x.RenderAmbientLight(this.GetSystem<AmbientLightSystem>(), ls.EnableSSAO))
                        .EnableIf(ls.EnableDirectionalLights, x => x.RenderDirectionalLights(this.GetSystem<DirectionalLightSystem>()))
                        .EnableIf(ls.EnablePointLights, x => x.RenderPointLights(this.GetSystem<PointLightSystem>()))
                        .EnableIf(ls.EnableShadowCastingLights, x => x.RenderShadowCastingLights(this.GetSystem<ShadowCastingLightSystem>()))
                        .EnableIf(ls.EnableSunLights, x => x.RenderSunlights(this.GetSystem<SunlightSystem>()));

                    modelPipeline.RenderLights(lightingPipeline);
                }

                var combineEffect = this.EffectFactory.Construct<CombineEffect>();
                var fxaaEffect = this.EffectFactory.Construct<FxaaEffect>();
                modelPipeline
                    .CombineDiffuseWithLighting(combineEffect)
                    .AntiAlias(fxaaEffect, settings.ModelSettings.FxaaFactor);

                pipeline.RenderModels(this.GetSystem<ModelSystem>(), modelPipeline);
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
                    .RenderTransparentParticles(this.GetSystem<AveragedParticleSystem>())
                    .RenderAdditiveParticles(this.GetSystem<AdditiveParticleSystem>());

                pipeline.RenderParticles(particlePipeline);
            }
        }

        private void AddDebug(RenderPipeline pipeline, RenderPipelineSettings settings)
        {
            pipeline
                .EnableIf(settings.EnableDebugLines, x => x.RenderDebugLines(this.GetSystem<LineSystem>()))
                .EnableIf(settings.Enable3DOutlines, x => x.Render3DOutline(this.GetSystem<BoundarySystem>()))
                .EnableIf(settings.Enable2DOutlines, x => x.Render2DOutline(this.GetSystem<BoundarySystem>()))
                .EnableIf(settings.EnableIcons, x => x.RenderIcons(this.GetSystem<IconSystem>()));
        }
    }
}
