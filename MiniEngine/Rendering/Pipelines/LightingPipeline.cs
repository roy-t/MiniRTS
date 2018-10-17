using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class LightingPipeline
    {
        private readonly List<ILightingPipelineStage> Stages;

        public LightingPipeline(GraphicsDevice device)
        {
            this.Device = device;
            this.Stages = new List<ILightingPipelineStage>();
        }

        public GraphicsDevice Device { get; }

        public void Add(ILightingPipelineStage stage)
        {
            this.Stages.Add(stage);
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer)
        {
            foreach (var stage in this.Stages)
            {
                stage.Execute(camera, gBuffer);
            }
        }

        public static LightingPipeline Create(GraphicsDevice device)
        {
            return new LightingPipeline(device);
        }
    }
}