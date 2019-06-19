using MiniEngine.Pipeline.Particles.Stages;
using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class RenderParticlesStageExtensions
    {
        public static RenderPipeline RenderParticles(
            this RenderPipeline pipeline,
            ParticlePipeline particlePipeline)
        {
            var stage = new RenderParticlesStage(particlePipeline);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}