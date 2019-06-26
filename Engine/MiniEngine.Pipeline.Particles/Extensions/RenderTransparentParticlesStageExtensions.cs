using MiniEngine.Pipeline.Particles.Stages;
using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class RenderTransparentParticlesStageExtensions
    {
        public static ParticlePipeline RenderTransparentParticles(
            this ParticlePipeline pipeline,
            AveragedParticleSystem particleSystem)
        {
            var stage = new RenderTransparentParticlesStage(pipeline.Device, particleSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}