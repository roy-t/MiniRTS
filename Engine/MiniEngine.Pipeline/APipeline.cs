using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Telemetry;
using System.Collections.Generic;

namespace MiniEngine.Pipeline
{
    public abstract class APipeline<T>
        where T : IPipelineInput
    {
        private readonly string StageGauge;

        protected readonly List<IPipelineStage<T>> Stages;
        private readonly IMeterRegistry MeterRegistry;

        public APipeline(GraphicsDevice device, IMeterRegistry meterRegistry, string name)
        {
            this.Device = device;
            this.MeterRegistry = meterRegistry;

            this.Stages = new List<IPipelineStage<T>>();

            this.StageGauge = $"{name}_stages_render_time";
            this.MeterRegistry.CreateGauge(this.StageGauge, "stage", "pass");
        }           

        public GraphicsDevice Device { get; }

        public void Add(IPipelineStage<T> stage) => this.Stages.Add(stage);

        public void Clear() => this.Stages.Clear();

        public void Execute(T input, string label)
        {
            for (var i = 0; i < this.Stages.Count; i++)
            {
                var stage = this.Stages[i];

                this.MeterRegistry.StartGauge(this.StageGauge);
                stage.Execute(input);
                this.MeterRegistry.StopGauge(this.StageGauge, stage.GetType().Name, label);
            }
        }
    }
}

