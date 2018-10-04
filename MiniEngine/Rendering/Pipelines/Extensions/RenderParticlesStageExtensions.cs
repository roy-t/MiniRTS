using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class RenderParticlesStageExtensions
    {
        public static Pipeline RenderParticles(
            this Pipeline pipeline,
            ParticleSystem particleSystem,
            ParticlePipeline particlePipeline)
        {
            var stage = new RenderParticlesStage(particleSystem, particlePipeline);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}