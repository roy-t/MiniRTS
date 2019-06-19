using MiniEngine.Pipeline.Debug.Stages;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Extensions
{
    public static class RenderIconsStageExtensions
    {
        public static RenderPipeline RenderIcons(this RenderPipeline pipeline, IconSystem iconSystem)
        {
            var stage = new RenderIconsStage(iconSystem, pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
