using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Shadows
{
    public sealed class ShadowPipeline
    {
        private readonly List<IShadowPipelineStage> Stages;

        public ShadowPipeline(GraphicsDevice device)
        {
            this.Stages = new List<IShadowPipelineStage>();
            this.Device = device;
        }

        public GraphicsDevice Device { get; }

        public void Add(IShadowPipelineStage stage) => this.Stages.Add(stage);

        public void Execute()
        {
            foreach(var stage in this.Stages)
            {
                stage.Execute();
            }
        }

        public static ShadowPipeline Create(GraphicsDevice device) => new ShadowPipeline(device);
    }
}
