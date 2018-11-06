using MiniEngine.Pipeline.Shadows.Stages;

namespace MiniEngine.Pipeline.Shadows.Extensions
{
    public static class RenderShadowsStageExtensions
    {
        public static RenderPipeline RenderShadows(this RenderPipeline pipeline, ShadowPipeline shadowPipeline)
        {
            var stage = new RenderShadowsStage(shadowPipeline);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
