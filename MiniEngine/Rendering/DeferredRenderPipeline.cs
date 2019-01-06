﻿using Microsoft.Xna.Framework.Graphics;
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
using MiniEngine.Pipeline.Shadows;
using MiniEngine.Pipeline.Shadows.Extensions;
using MiniEngine.Pipeline.Shadows.Systems;
using MiniEngine.Pipeline.Systems;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Telemetry;
using MiniEngine.Units;

namespace MiniEngine.Rendering
{
    public sealed class DeferredRenderPipeline
    {
        private readonly GBuffer GBuffer;
        private readonly RenderPipeline Pipeline;
        private readonly RenderPipelineStageInput Input;

        public DeferredRenderPipeline(
            GraphicsDevice device,
            ShadowMapSystem shadowMapSystem,
            ModelSystem modelSystem,
            ParticleSystem particleSystem,
            CombineEffect combineEffect,
            FxaaEffect fxaaEffect,
            AmbientLightSystem ambientLightSystem,
            DirectionalLightSystem directionalLightSystem,
            PointLightSystem pointLightSystem,
            CascadedShadowMapSystem cascadedShadowMapSystem,
            ShadowCastingLightSystem shadowCastingLightSystem,
            SunlightSystem sunlightSystem,
            DebugRenderSystem debugRenderSystem,
            IMeterRegistry meterRegistry)
        {
            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;         
            this.GBuffer = new GBuffer(device, width, height);

            this.Input = new RenderPipelineStageInput();

            var shadowPipeline =
                ShadowPipeline.Create(device, meterRegistry)
                              .RenderShadowMaps(shadowMapSystem);

            var lightingPipeline =
                LightingPipeline.Create(device, meterRegistry)                
                                .ClearLightTargets()
                                .RenderAmbientLight(ambientLightSystem)
                                .RenderDirectionalLights(directionalLightSystem)
                                .RenderPointLights(pointLightSystem)
                                .RenderShadowCastingLights(shadowCastingLightSystem)
                                .RenderSunlights(sunlightSystem);

            var modelPipeline =
                ModelPipeline.Create(device, meterRegistry)
                             .ClearModelRenderTargets()
                             .RenderModelBatch()
                             .RenderLights(lightingPipeline)
                             .CombineDiffuseWithLighting(combineEffect)
                             .AntiAlias(fxaaEffect, 2.0f);

            var particlePipeline =
                ParticlePipeline.Create(device, meterRegistry)
                                .ClearParticleRenderTargets()
                                .RenderWeightedParticles(particleSystem);            

            this.Pipeline =
                RenderPipeline.Create(device, meterRegistry)
                        .ClearRenderTargetSet()
                        .UpdateSystem(cascadedShadowMapSystem)
                        .UpdateSystem(particleSystem)
                        .RenderShadows(shadowPipeline)
                        .RenderModels(modelSystem, modelPipeline)
                        .RenderParticles(particleSystem, particlePipeline)
                        .Render3DDebugOverlay(debugRenderSystem)
                        .Render2DDebugOverlay(debugRenderSystem);

        }
        
        public RenderTarget2D Render(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Input.Update(camera, elapsed, this.GBuffer, "render");
            this.Pipeline.Execute(this.Input);
            return this.GBuffer.FinalTarget;
        }

        public GBuffer GetGBuffer() => this.GBuffer;
    }
}