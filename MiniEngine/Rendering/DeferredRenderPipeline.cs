using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Pipelines;
using MiniEngine.Rendering.Pipelines.Extensions;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering
{
    public sealed class DeferredRenderPipeline
    {
        private readonly Pipeline Pipeline;
        private readonly RenderTarget2D PostProcessTarget;
        private readonly GBuffer GBuffer;

        public DeferredRenderPipeline(GraphicsDevice device, ShadowMapSystem shadowMapSystem, ModelSystem modelSystem,
                                      ParticleSystem particleSystem, CombineEffect combineEffect, PostProcessEffect postProcessEffect,
                                      AmbientLightSystem ambientLightSystem, DirectionalLightSystem directionalLightSystem,
                                      PointLightSystem pointLightSystem, ShadowCastingLightSystem shadowCastingLightSystem,
                                      SunlightSystem sunlightSystem, DebugRenderSystem debugRenderSystem)
        {
            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;

            this.PostProcessTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            this.GBuffer = new GBuffer(device, width, height);

            var combineTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            var lightingPipeline =
                LightingPipeline.Create(device)
                                .ClearToAmbientLight(ambientLightSystem)
                                .RenderDirectionalLights(directionalLightSystem)
                                .RenderPointLights(pointLightSystem)
                                .RenderShadowCastingLights(shadowCastingLightSystem)
                                .RenderSunlights(sunlightSystem);

            var modelPipeline =
                ModelPipeline.Create(device)
                             .Clear(this.GBuffer.DiffuseTarget, ClearOptions.Target, Color.TransparentBlack, 1, 0)
                             .Clear(this.GBuffer.NormalTarget, new Color(0.5f, 0.5f, 0.5f, 0.0f))
                             .Clear(this.GBuffer.DepthTarget, Color.TransparentBlack)
                             .Clear(combineTarget, Color.TransparentBlack)
                             .RenderModelBatch(this.GBuffer)
                             .Render3DDebugOverlay(debugRenderSystem, this.GBuffer)
                             .RenderLights(lightingPipeline, this.GBuffer)                             
                             .CombineDiffuseWithLighting(combineEffect, combineTarget, this.GBuffer)
                             .AntiAlias(postProcessEffect, combineTarget, this.PostProcessTarget, this.GBuffer, 2.0f);

            var particlePipeline =
                ParticlePipeline.Create(device)
                                .Clear(this.GBuffer.DiffuseTarget, ClearOptions.Target, Color.TransparentBlack, 1, 0)
                                .Clear(this.GBuffer.NormalTarget, new Color(0.5f, 0.5f, 0.5f, 0.0f))
                                .Clear(this.GBuffer.DepthTarget, Color.TransparentBlack)
                                .Clear(combineTarget, Color.TransparentBlack)
                                .RenderParticleBatch(this.GBuffer)
                                .RenderLights(lightingPipeline, this.GBuffer)
                                .CombineDiffuseWithLighting(combineEffect, combineTarget, this.GBuffer)
                                .AntiAlias(postProcessEffect, combineTarget, this.PostProcessTarget, this.GBuffer, 2.0f);
                            
            // TODO: we could move the anti-alias stage to the end of the normal pipeline
            // if we copy the deferred and normal result of each sub pipeline

            this.Pipeline =
                Pipeline.Create(device)
                        .Clear(this.GBuffer.DiffuseTarget, Color.TransparentBlack)
                        .Clear(this.PostProcessTarget, Color.Black)
                        .UpdateCascades(sunlightSystem)
                        .RenderShadowMaps(shadowMapSystem)
                        .RenderModels(modelSystem, modelPipeline)
                        .RenderParticles(particleSystem, particlePipeline)
                        .Render2DDebugOverlay(debugRenderSystem, this.PostProcessTarget);            
        }

        public RenderTarget2D Render(PerspectiveCamera camera)
        {
            this.Pipeline.Execute(camera);
            return this.PostProcessTarget;
        }

        public RenderTarget2D[] GetIntermediateRenderTargets() => new[]
        {
            this.GBuffer.DiffuseTarget,
            this.GBuffer.NormalTarget,
            this.GBuffer.DepthTarget,
            this.GBuffer.LightTarget,
        };
    }
}
