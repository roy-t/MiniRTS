namespace MiniEngine.Pipeline.Shadows.Stages
{
    public sealed class RenderShadowsStage : IPipelineStage<RenderPipelineInput>
    {
        private readonly ShadowPipeline ShadowPipeline;

        private readonly ShadowPipelineInput Input;

        public RenderShadowsStage(ShadowPipeline shadowPipeline)
        {
            this.ShadowPipeline = shadowPipeline;
            this.Input = new ShadowPipelineInput();
        }

        public void Execute(RenderPipelineInput input)
        {
            this.Input.Update(input.GBuffer, input.Camera, "shadows");
            this.ShadowPipeline.Execute(this.Input);
        }
    }
}
