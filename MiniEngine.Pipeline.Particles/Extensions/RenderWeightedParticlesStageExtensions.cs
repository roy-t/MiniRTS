using MiniEngine.Pipeline.Particles.Stages;
using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class RenderWeightedParticlesStageExtensions
    {
        public static ParticlePipeline RenderWeightedParticles(
            this ParticlePipeline pipeline,
            ParticleSystem particleSystem)
        {
            var stage = new RenderWeightedParticlesStage(pipeline.Device, particleSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}