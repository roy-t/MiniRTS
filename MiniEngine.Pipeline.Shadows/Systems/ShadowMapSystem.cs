using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Pipeline.Particles.Systems;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Telemetry;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Shadows.Systems
{
    public sealed class ShadowMapSystem : ISystem
    {
        private const string ShadowMapCounter = "shadow_pipeline_shadow_map_counter";
        private const string ShadowMapTotal = "shadow_pipeline_total_render_time";
        private const string ShadowMapStep = "shadow_pipeline_step_render_time";

        private const int DefaultResolution = 1024;

        private readonly GraphicsDevice Device;
        private readonly EntityLinker EntityLinker;
        private readonly ModelSystem ModelSystem;
        private readonly TransparentParticleSystem ParticleSystem;
        private readonly List<ShadowMap> ShadowMaps;
        private readonly IMeterRegistry MeterRegistry;

        public ShadowMapSystem(
            GraphicsDevice device,
            EntityLinker entityLinker,
            ModelSystem modelSystem,
            TransparentParticleSystem particleSystem,
            IMeterRegistry meterRegistry)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
            this.ModelSystem = modelSystem;
            this.ParticleSystem = particleSystem;
            this.MeterRegistry = meterRegistry;

            this.ShadowMaps = new List<ShadowMap>();

            this.MeterRegistry.CreateGauge(ShadowMapCounter);
            this.MeterRegistry.CreateGauge(ShadowMapTotal);
            this.MeterRegistry.CreateGauge(ShadowMapStep, "step");
        }

        public bool Contains(Entity entity) => false;

        public string Describe(Entity entity) => "";

        public void Remove(Entity entity) => this.EntityLinker.RemoveComponents<ShadowMap>(entity);

        public void Add(Entity entity, IViewPoint viewPoint, int resolution = DefaultResolution)
            => this.EntityLinker.AddComponent(entity, new ShadowMap(this.Device, resolution, viewPoint));

        public void Add(Entity entity, RenderTarget2D depthMapArray, RenderTarget2D colorMapArray, int index, IViewPoint viewPoint)
            => this.EntityLinker.AddComponent(entity, new ShadowMap(depthMapArray, colorMapArray, index, viewPoint));

        public void RenderShadowMaps(GBuffer gBuffer)
        {
            this.MeterRegistry.SetGauge(ShadowMapCounter, this.ShadowMaps.Count);
            this.MeterRegistry.StartGauge(ShadowMapTotal);

            this.ShadowMaps.Clear();
            this.EntityLinker.GetComponents(this.ShadowMaps);

            foreach (var shadowMap in this.ShadowMaps)
            {
                var modelBatchList = this.ModelSystem.ComputeBatches(shadowMap.ViewPoint);

                this.MeterRegistry.StartGauge(ShadowMapStep);
                {
                    // First compute the shadow maps
                    this.Device.SetRenderTarget(shadowMap.DepthMap, shadowMap.Index);
                    this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                    using (this.Device.ShadowMapState())
                    {
                        modelBatchList.OpaqueBatch.Draw(RenderEffectTechniques.ShadowMap);
                    }
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

                    using (this.Device.AlphaBlendOccluderState())
                    {
                        foreach (var batch in modelBatchList.TransparentBatches)
                        {
                            batch.Draw(RenderEffectTechniques.Textured);
                        }
                    }

                }
                this.MeterRegistry.StopGauge(ShadowMapStep, "transparent");

                // TODO: this will not clear the diffuse target so it will bug
                //this.MeterRegistry.StartGauge(ShadowMapStep);
                //{
                //    // Read the depth buffer and render occluding particles
                //    this.Device.SetRenderTargets(gBuffer.DiffuseTarget, gBuffer.ParticleTarget);
                //    this.ParticleSystem.RenderWeights(shadowMap.ViewPoint);

                //    this.Device.SetRenderTarget(shadowMap.ColorMap, shadowMap.Index);
                //    this.ParticleSystem.RenderParticles(gBuffer.DiffuseTarget, gBuffer.ParticleTarget);
                //}
                //this.MeterRegistry.StopGauge(ShadowMapStep, "particles");

                // TODO: if a particle is behind a stained glass window
                // it will shadow as if its in front of it. 
            }

            this.MeterRegistry.StopGauge(ShadowMapTotal);
        }
    }
}