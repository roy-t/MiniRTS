using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Telemetry;
using MiniEngine.Units;

namespace MiniEngine.Pipeline
{
    public sealed class RenderPipeline
    {
        private readonly IMeterRegistry MeterRegistry;

        private readonly List<IPipelineStage> Stages;
        private readonly List<Gauge> StageGauges;
        private readonly Gauge TotalGauge;

        public RenderPipeline(GraphicsDevice device, IMeterRegistry meterRegistry)
        {
            this.Device = device;
            this.MeterRegistry = meterRegistry;
            this.Stages = new List<IPipelineStage>();
            this.StageGauges = new List<Gauge>();
            this.TotalGauge = meterRegistry.CreateGauge("render_pipeline_total_render_time");
        }

        public GraphicsDevice Device { get; }

        public void Add(IPipelineStage stage)
        {
            this.Stages.Add(stage);                       
            this.StageGauges.Add(this.MeterRegistry.CreateGauge("render_pipeline_stages_render_time", new Tag("stage", stage.GetType().Name)));
        }

        public void Execute(PerspectiveCamera camera, Seconds elapsed)
        {
            this.TotalGauge.BeginMeasurement();
            for(var i = 0; i < this.Stages.Count; i++)
            {
                var stage = this.Stages[i];
                var gauge = this.StageGauges[i];
                
                gauge.BeginMeasurement();
                {
                    stage.Execute(camera, elapsed);
                }
                gauge.EndMeasurement();
            }
            this.TotalGauge.EndMeasurement();
        }

        public static RenderPipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry) => new RenderPipeline(device, meterRegistry);
    }
}