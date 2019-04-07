using MiniEngine.Pipeline.Particles.Stages;
using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class RenderAdditiveParticlesStageExtensions
    {
        public static ParticlePipeline RenderAdditiveParticles(
            this ParticlePipeline pipeline,
            AdditiveParticleSystem particleSystem)
        {
            var stage = new RenderAdditiveParticlesStage(pipeline.Device, particleSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}