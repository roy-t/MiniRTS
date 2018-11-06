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
        private readonly List<Gauge> Gauges;

        public RenderPipeline(GraphicsDevice device, IMeterRegistry meterRegistry)
        {
            this.Device = device;
            this.MeterRegistry = meterRegistry;
            this.Stages = new List<IPipelineStage>();
            this.Gauges = new List<Gauge>();
        }

        public GraphicsDevice Device { get; }

        public void Add(IPipelineStage stage)
        {
            this.Stages.Add(stage);
            //this.Gauges.Add(this.MeterRegistry.CreateGauge($"RenderPipeline.{stage.GetType().Name}.Total"));
            this.Gauges.Add(this.MeterRegistry.CreateGauge(stage.GetType().Name));
        }

        public void Execute(PerspectiveCamera camera, Seconds elapsed)
        {
            for(var i = 0; i < this.Stages.Count; i++)
            {
                var stage = this.Stages[i];
                var gauge = this.Gauges[i];
                gauge.Measure(() => stage.Execute(camera, elapsed));
            }
        }

        public static RenderPipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry) => new RenderPipeline(device, meterRegistry);
    }
}