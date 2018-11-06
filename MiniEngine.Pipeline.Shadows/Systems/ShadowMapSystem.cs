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
        private const int DefaultResolution = 1024;

        private readonly GraphicsDevice Device;
        private readonly ModelSystem ModelSystem;
        private readonly ParticleSystem ParticleSystem;
        private readonly Dictionary<Entity, ShadowMap> ShadowMaps;

        private readonly Counter ShadowMapCounter;
        private readonly Counter ShadowMapCascadesCounter;
        private readonly Gauge ShadowMapGauge;
        private readonly Gauge OpaqueGauge;
        private readonly Gauge TransparentGauge;
        private readonly Gauge ParticleGauge;

        public ShadowMapSystem(
            IMeterRegistry meterRegistry,
            GraphicsDevice device,
            ModelSystem modelSystem,
            ParticleSystem particleSystem)
        {
            this.Device = device;
            this.ModelSystem = modelSystem;
            this.ParticleSystem = particleSystem;

            this.ShadowMapCounter = meterRegistry.CreateCounter("shadow_pipeline_shadow_map_counter");
            this.ShadowMapCascadesCounter = meterRegistry.CreateCounter("shadow_pipeline_shadow_map_cascades_counter");
            this.ShadowMapGauge = meterRegistry.CreateGauge("shadow_pipeline_total_render_time");
            this.OpaqueGauge = meterRegistry.CreateGauge("shadow_pipeline_step_render_time", new Tag("step", "opaque"));
            this.TransparentGauge = meterRegistry.CreateGauge("shadow_pipeline_step_render_time", new Tag("step", "transparent"));
            this.ParticleGauge = meterRegistry.CreateGauge("shadow_pipeline_step_render_time", new Tag("step", "particles"));

            this.ShadowMaps = new Dictionary<Entity, ShadowMap>();
        }

        public bool Contains(Entity entity) => this.ShadowMaps.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var shadowMap = this.ShadowMaps[entity];
            return $"shadow map, dimensions: {shadowMap.DepthMap.Width}x{shadowMap.DepthMap.Height}";
        }

        public void Remove(Entity entity) => this.ShadowMaps.Remove(entity);

        public void Add(Entity entity, IViewPoint viewPoint, int resolution = DefaultResolution) => this.ShadowMaps.Add(entity, new ShadowMap(this.Device, resolution, 1, viewPoint));

        public void Add(Entity entity, IViewPoint[] viewPoints, int cascades, int resolution = DefaultResolution) => this.ShadowMaps.Add(entity, new ShadowMap(this.Device, resolution, cascades, viewPoints));

        public ShadowMap Get(Entity entity) => this.ShadowMaps[entity];

        public void RenderShadowMaps()
        {
            this.ShadowMapCounter.IncreaseWith(this.ShadowMaps.Count);
            this.ShadowMapGauge.BeginMeasurement();
            foreach (var shadowMap in this.ShadowMaps.Values)
            {
                this.ShadowMapCascadesCounter.IncreaseWith(shadowMap.Cascades);
                for (var i = 0; i < shadowMap.Cascades; i++)
                {                                        
                    var modelBatchList = this.ModelSystem.ComputeBatches(shadowMap.ViewPoints[i]);
                    var particleBatchList = this.ParticleSystem.ComputeBatches(shadowMap.ViewPoints[i]);

                    this.OpaqueGauge.BeginMeasurement();
                    {
                        // First compute the shadow maps
                        this.Device.SetRenderTarget(shadowMap.DepthMap, i);
                        this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
                        
                        using (this.Device.ShadowMapState())
                        {
                            modelBatchList.OpaqueBatch.Draw(RenderEffectTechniques.ShadowMap);
                        }
                    }
                    this.OpaqueGauge.EndMeasurement();


                    this.TransparentGauge.BeginMeasurement();
                    {
                        // Read the depth buffer and render objects that are partially
                        // occluding, like a stained glass window
                        this.Device.SetRenderTarget(shadowMap.ColorMap, i);
                        this.Device.Clear(ClearOptions.Target, Color.White, 1.0f, 0);

                        
                        using (this.Device.AlphaBlendOccluderState())
                        {
                            foreach (var batch in modelBatchList.TransparentBatches)
                            {
                                batch.Draw(RenderEffectTechniques.Textured);
                            }
                        }

                    }
                    this.TransparentGauge.EndMeasurement();

                    this.ParticleGauge.BeginMeasurement();
                    {
                        // Read the depth buffer and render occluding particles
                        using (this.Device.AdditiveBlendOccluderState())
                        {
                            foreach (var batch in particleBatchList.Batches)
                            {
                                batch.Draw(RenderEffectTechniques.GrayScale);
                            }
                        }
                    }
                    this.ParticleGauge.EndMeasurement();

                    // TODO: if a particle is behind a stained glass window
                    // it will shadow as if its in front of it. 
                }
            }
            this.ShadowMapGauge.EndMeasurement();
        }
    }
}