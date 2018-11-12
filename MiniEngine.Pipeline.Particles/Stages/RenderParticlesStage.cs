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
            var particleBatchList = this.ParticleSystem.ComputeBatches(input.Camera);
            for(var i = 0; i < particleBatchList.Batches.Count; i++)
            {
                var batch = particleBatchList.Batches[i];
                this.Input.Update(input.Camera, batch, input.GBuffer, $"transparent_{i}");
                this.ParticlePipeline.Execute(this.Input);
            }
        }
    }
}