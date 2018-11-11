using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Telemetry;
using MiniEngine.Units;

namespace MiniEngine.Pipeline
{
    public sealed class RenderPipeline
    {
        private const string StageGauge = "render_pipeline_stages_render_time";
        private const string StageTag = "stage";
        private const string TotalGauge = "render_pipeline_total_render_time";


        private readonly List<IPipelineStage> Stages;
        private readonly IMeterRegistry MeterRegistry;

        public RenderPipeline(GraphicsDevice device, IMeterRegistry meterRegistry)
        {
            this.Device = device;
            this.MeterRegistry = meterRegistry;
            this.Stages = new List<IPipelineStage>();

            this.MeterRegistry.CreateGauge(StageGauge, StageTag);
            this.MeterRegistry.CreateGauge(TotalGauge);
        }

        public GraphicsDevice Device { get; }

        public void Add(IPipelineStage stage) => this.Stages.Add(stage);

        public void Execute(PerspectiveCamera camera, Seconds elapsed)
        {
            this.MeterRegistry.StartGauge(TotalGauge);
            for(var i = 0; i < this.Stages.Count; i++)
            {
                var stage = this.Stages[i];
                this.MeterRegistry.StartGauge(StageGauge);                
                stage.Execute(camera, elapsed);
                this.MeterRegistry.StopGauge(StageGauge, stage.GetType().Name);
            }
            this.MeterRegistry.StopGauge(TotalGauge);
        }

        public static RenderPipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry) => new RenderPipeline(device, meterRegistry);
    }
}