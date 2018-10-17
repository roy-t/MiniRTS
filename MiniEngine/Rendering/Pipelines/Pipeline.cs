using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class Pipeline
    {
        private readonly List<IPipelineStage> Stages;

        public Pipeline(GraphicsDevice device)
        {
            this.Device = device;
            this.Stages = new List<IPipelineStage>();
        }

        public GraphicsDevice Device { get; }

        public void Add(IPipelineStage stage) => this.Stages.Add(stage);

        public void Execute(PerspectiveCamera camera, Seconds elapsed)
        {
            foreach (var stage in this.Stages)
            {
                stage.Execute(camera, elapsed);
            }
        }

        public static Pipeline Create(GraphicsDevice device) => new Pipeline(device);
    }
}