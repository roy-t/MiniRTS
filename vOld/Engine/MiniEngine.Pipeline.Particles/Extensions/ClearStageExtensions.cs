using MiniEngine.Pipeline.Particles.Stages;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class ClearStageExtensions
    {
        public static ParticlePipeline ClearParticleRenderTargets(this ParticlePipeline pipeline)
        {
            pipeline.Add(new ClearStage());
            return pipeline;
        }
    }
}
