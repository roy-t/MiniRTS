using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Pipeline
{
    public sealed class RenderPipeline
    {
        private readonly List<IPipelineStage> Stages;

        public RenderPipeline(GraphicsDevice device)
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

        public static RenderPipeline Create(GraphicsDevice device) => new RenderPipeline(device);
    }
}