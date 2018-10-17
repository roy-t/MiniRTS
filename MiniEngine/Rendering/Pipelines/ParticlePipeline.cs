using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class ParticlePipeline
    {
        private readonly List<IParticlePipelineStage> Stages;

        public ParticlePipeline(GraphicsDevice device)
        {
            this.Device = device;
            this.Stages = new List<IParticlePipelineStage>();
        }

        public GraphicsDevice Device { get; }

        public void Add(IParticlePipelineStage stage) => this.Stages.Add(stage);

        public void Execute(PerspectiveCamera camera, ParticleBatchList particleBatchList)
        {
            foreach (var batch in particleBatchList.Batches)
            {
                foreach (var stage in this.Stages)
                {
                    stage.Execute(camera, batch);
                }
            }
        }

        public static ParticlePipeline Create(GraphicsDevice device) => new ParticlePipeline(device);
    }
}