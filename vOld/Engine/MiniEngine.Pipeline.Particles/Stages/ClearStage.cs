namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class ClearStage : IPipelineStage<ParticlePipelineInput>
    {
        public void Execute(ParticlePipelineInput input)
        {
            input.GBuffer.SetClearDiffuseTargetColorOnly();
            input.GBuffer.SetClearParticleTarget();
        }
    }
}
