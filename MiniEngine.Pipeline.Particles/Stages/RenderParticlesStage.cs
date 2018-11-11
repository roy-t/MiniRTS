using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class RenderParticlesStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly ParticlePipeline ParticlePipeline;
        private readonly ParticleSystem ParticleSystem;

        public RenderParticlesStage(ParticleSystem particleSystem, ParticlePipeline particlePipeline)
        {
            this.ParticleSystem = particleSystem;
            this.ParticlePipeline = particlePipeline;
        }

        public void Execute(RenderPipelineStageInput input)
        {
            var particleBatchList = this.ParticleSystem.ComputeBatches(input.Camera);
            this.ParticlePipeline.Execute(input.Camera, particleBatchList);
        }
    }
}