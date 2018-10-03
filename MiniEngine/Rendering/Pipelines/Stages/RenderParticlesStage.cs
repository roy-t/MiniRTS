using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class RenderParticlesStage : IPipelineStage
    {
        private readonly ParticleSystem ParticleSystem;
        private readonly ParticlePipeline ParticlePipeline;

        public RenderParticlesStage(ParticleSystem particleSystem, ParticlePipeline particlePipeline)
        {
            this.ParticleSystem = particleSystem;
            this.ParticlePipeline = particlePipeline;
        }

        public void Execute(PerspectiveCamera camera)
        {
            var particleBatchList = this.ParticleSystem.ComputeBatches(camera);
            this.ParticlePipeline.Execute(camera, particleBatchList);
        }
    }
}
