using MiniEngine.Pipeline;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class RenderParticlesStage : IPipelineStage
    {
        private readonly ParticlePipeline ParticlePipeline;
        private readonly ParticleSystem ParticleSystem;

        public RenderParticlesStage(ParticleSystem particleSystem, ParticlePipeline particlePipeline)
        {
            this.ParticleSystem = particleSystem;
            this.ParticlePipeline = particlePipeline;
        }

        public void Execute(PerspectiveCamera camera, Seconds seconds)
        {
            var particleBatchList = this.ParticleSystem.ComputeBatches(camera);
            this.ParticlePipeline.Execute(camera, particleBatchList);
        }
    }
}