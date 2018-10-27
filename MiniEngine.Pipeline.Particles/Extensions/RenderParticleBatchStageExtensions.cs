using MiniEngine.Pipeline.Particles.Stages;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class RenderParticleBatchStageExtensions
    {
        public static ParticlePipeline RenderParticleBatch(this ParticlePipeline pipeline, GBuffer gBuffer)
        {
            var stage = new RenderParticleBatchStage(pipeline.Device, gBuffer);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}