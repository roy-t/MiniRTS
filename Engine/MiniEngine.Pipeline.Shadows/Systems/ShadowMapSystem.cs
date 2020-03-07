using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Telemetry;

namespace MiniEngine.Pipeline.Shadows.Systems
{
    public sealed class ShadowMapSystem : ISystem
    {
        private const string ShadowMapCounter = "shadow_pipeline_shadow_map_counter";
        private const string ShadowMapTotal = "shadow_pipeline_total_render_time";
        private const string ShadowMapStep = "shadow_pipeline_step_render_time";

        private readonly GraphicsDevice Device;
        private readonly ModelSystem ModelSystem;
        private readonly IComponentContainer<ShadowMap> ShadowMaps;
        private readonly IMeterRegistry MeterRegistry;

        private readonly TextureCube NullSkybox;

        public ShadowMapSystem(
            GraphicsDevice device,
            IComponentContainer<ShadowMap> shadowMaps,
            ModelSystem modelSystem,
            IMeterRegistry meterRegistry)
        {
            this.Device = device;
            this.ShadowMaps = shadowMaps;
            this.ModelSystem = modelSystem;
            this.MeterRegistry = meterRegistry;

            this.MeterRegistry.CreateGauge(ShadowMapCounter);
            this.MeterRegistry.CreateGauge(ShadowMapTotal);
            this.MeterRegistry.CreateGauge(ShadowMapStep, "step");

            this.NullSkybox = new TextureCube(device, 1, false, SurfaceFormat.Color);
            this.NullSkybox.SetData(CubeMapFace.PositiveX, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.NegativeX, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.PositiveY, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.NegativeY, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.PositiveZ, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.NegativeZ, new Color[] { Color.White });
        }

        public void RenderShadowMaps()
        {
            this.MeterRegistry.SetGauge(ShadowMapCounter, this.ShadowMaps.Count);
            this.MeterRegistry.StartGauge(ShadowMapTotal);

            for (var iMap = 0; iMap < this.ShadowMaps.Count; iMap++)
            {
                var shadowMap = this.ShadowMaps[iMap];
                var modelBatchList = this.ModelSystem.ComputeBatches(shadowMap.ViewPoint, this.NullSkybox);

                this.MeterRegistry.StartGauge(ShadowMapStep);
                {
                    // First compute the shadow maps
                    this.Device.SetRenderTarget(shadowMap.DepthMap, shadowMap.Index);
                    this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                    this.Device.ShadowMapState();

                    modelBatchList.OpaqueBatch.Draw(RenderEffectTechniques.ShadowMap);

                }
                this.MeterRegistry.StopGauge(ShadowMapStep, "opaque");

                // TODO: Color maps need to be redone
                // - If an occluder is between the light and the transparent object, it will also get the transparent object painted on it
                // - They are hard to mix with particles in the current state
                // - Expensive since we draw so much double?
                // - Might benefit from better filtering


                // TODO: since this doesn't use the depth map, will it sometimes show colors that should not be visible?
                // for example those that are in front of a thing?
                this.MeterRegistry.StartGauge(ShadowMapStep);
                {
                    // Read the depth buffer and render objects that are partially
                    // occluding, like a stained glass window
                    this.Device.SetRenderTarget(shadowMap.ColorMap, shadowMap.Index);
                    this.Device.Clear(ClearOptions.Target, Color.White, 1.0f, 0);

                    this.Device.AlphaBlendOccluderState();

                    for (var iBatch = 0; iBatch < modelBatchList.TransparentBatches.Count; iBatch++)
                    {
                        var batch = modelBatchList.TransparentBatches[iBatch];
                        batch.Draw(RenderEffectTechniques.Textured);
                    }
                }
                this.MeterRegistry.StopGauge(ShadowMapStep, "transparent");
            }

            this.MeterRegistry.StopGauge(ShadowMapTotal);
        }
    }
}