using MiniEngine.Pipeline;
using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class RenderShadowMapsStageExtension
    {
        public static RenderPipeline RenderShadowMaps(this RenderPipeline pipeline, ShadowMapSystem shadowMapSystem)
        {
            var stage = new RenderShadowMapsStage(shadowMapSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}