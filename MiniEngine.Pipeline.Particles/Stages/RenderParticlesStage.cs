using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class RenderParticlesStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly ParticlePipeline ParticlePipeline;
        private readonly ParticleSystem ParticleSystem;

        private readonly ParticlePipelineInput Input;

        public RenderParticlesStage(ParticleSystem particleSystem, ParticlePipeline particlePipeline)
        {
            this.ParticleSystem = particleSystem;
            this.ParticlePipeline = particlePipeline;

            this.Input = new ParticlePipelineInput();
        }

        public void Execute(RenderPipelineStageInput input)
        {
            this.Input.Update(input.Camera, input.GBuffer, $"particles");
            this.ParticlePipeline.Execute(this.Input);
        }
    }
}