using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

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

        public void Execute(PerspectiveCamera camera, Seconds elapsed)
        {
            var particleBatchList = this.ParticleSystem.ComputeBatches(camera, elapsed);
            this.ParticlePipeline.Execute(camera, particleBatchList, elapsed);
        }
    }
}
