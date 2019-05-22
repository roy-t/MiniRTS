namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class ClearStage : IPipelineStage<ModelPipelineInput>
    {
        public void Execute(ModelPipelineInput input)
        {
            input.GBuffer.SetClearDiffuseTargetColorOnly();
            input.GBuffer.SetClearNormalTarget();
            
            if (input.Pass.Type == PassType.Opaque)
            {
                input.GBuffer.SetClearDepthTarget();
            }

            input.GBuffer.SetClearCombineTarget();
        }
    }
}
