namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class ClearStage : IPipelineStage<LightingPipelineInput>
    {
        public void Execute(LightingPipelineInput input)
        {
            input.GBuffer.SetClearBlurTarget();
            input.GBuffer.SetClearLightTarget();
        }
    }
}
