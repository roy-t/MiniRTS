namespace MiniEngine.Pipeline.Shadows.Stages
{
    public sealed class RenderShadowsStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly ShadowPipeline ShadowPipeline;

        public RenderShadowsStage(ShadowPipeline shadowPipeline)
        {
            this.ShadowPipeline = shadowPipeline;
        }

        public void Execute(RenderPipelineStageInput _) => this.ShadowPipeline.Execute();
    }
}
