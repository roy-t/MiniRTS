using MiniEngine.Pipeline;
using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class RenderParticlesStageExtensions
    {
        public static RenderPipeline RenderParticles(
            this RenderPipeline pipeline,
            ParticleSystem particleSystem,
            ParticlePipeline particlePipeline)
        {
            var stage = new RenderParticlesStage(particleSystem, particlePipeline);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}