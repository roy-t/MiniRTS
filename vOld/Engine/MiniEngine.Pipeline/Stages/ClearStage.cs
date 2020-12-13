namespace MiniEngine.Pipeline.Stages
{
    public sealed class ClearStage : IPipelineStage<RenderPipelineInput>
    {
        public void Execute(RenderPipelineInput input)
            => input.GBuffer.ClearAllTargets();
    }
}
