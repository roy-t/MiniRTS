namespace MiniEngine.Pipeline.Shadows.Stages
{
    public sealed class RenderShadowsStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly ShadowPipeline ShadowPipeline;

        private readonly ShadowPipelineInput Input;

        public RenderShadowsStage(ShadowPipeline shadowPipeline)
        {
            this.ShadowPipeline = shadowPipeline;
            this.Input = new ShadowPipelineInput();
        }

        public void Execute(RenderPipelineStageInput input)
        {
            this.Input.Update(input.Camera, "shadows");
            this.ShadowPipeline.Execute(this.Input);
        }
    }
}
