using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Pipeline.Particles.Systems;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Telemetry;

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
        private readonly ParticleSystem ParticleSystem;
        private readonly List<ShadowMap> ShadowMaps;
        private readonly IMeterRegistry MeterRegistry;

        public ShadowMapSystem(            
            GraphicsDevice device,
            EntityLinker entityLinker,
            ModelSystem modelSystem,
            ParticleSystem particleSystem,
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

        public void RenderShadowMaps()
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

                // TODO: enable particles for shadows again
                //var particleBatchList = this.ParticleSystem.ComputeBatches(shadowMap.ViewPoint);
                //this.MeterRegistry.StartGauge(ShadowMapStep);
                //{
                //    // Read the depth buffer and render occluding particles
                //    using (this.Device.AdditiveBlendOccluderState())
                //    {
                //        foreach (var batch in particleBatchList.Batches)
                //        {
                //            batch.Draw(RenderEffectTechniques.GrayScale);
                //        }
                //    }
                //}
                //this.MeterRegistry.StopGauge(ShadowMapStep, "particles");

                // TODO: if a particle is behind a stained glass window
                // it will shadow as if its in front of it. 
            }
            
            this.MeterRegistry.StopGauge(ShadowMapTotal);
        }
    }
}