using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;

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

        public void Add(IPipelineStage stage)
        {
            this.Stages.Add(stage);
        }

        public void Execute(PerspectiveCamera camera)
        {
            foreach (var stage in this.Stages)
            {
                stage.Execute(camera);
            }
        }

        public static Pipeline Create(GraphicsDevice device)
        {
            return new Pipeline(device);
        }       
    }
}
