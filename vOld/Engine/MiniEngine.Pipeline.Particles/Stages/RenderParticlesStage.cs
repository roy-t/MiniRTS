using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class RenderParticlesStage : IPipelineStage<RenderPipelineInput>
    {
        private readonly ParticlePipeline ParticlePipeline;

        private readonly ParticlePipelineInput Input;

        public RenderParticlesStage(ParticlePipeline particlePipeline)
        {
            this.ParticlePipeline = particlePipeline;
            this.Input = new ParticlePipelineInput();
        }

        public void Execute(RenderPipelineInput input)
        {
            this.Input.Update(input.Camera, input.GBuffer, input.Pass);
            this.ParticlePipeline.Execute(this.Input, "particles");
        }
    }
}