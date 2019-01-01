using MiniEngine.Pipeline.Particles.Stages;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class ClearStageExtensions
    {
        public static ParticlePipeline ClearParticleRenderTargets(this ParticlePipeline pipeline)
        {
            var stage = new ClearStage(pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
