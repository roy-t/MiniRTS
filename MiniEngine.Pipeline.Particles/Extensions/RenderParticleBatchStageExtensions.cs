using MiniEngine.Pipeline.Particles.Stages;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class RenderParticleBatchStageExtensions
    {
        public static ParticlePipeline RenderParticleBatch(this ParticlePipeline pipeline)
        {
            var stage = new RenderParticleBatchStage(pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}