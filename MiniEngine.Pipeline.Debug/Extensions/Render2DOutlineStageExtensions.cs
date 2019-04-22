using MiniEngine.Pipeline.Debug.Stages;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Extensions
{
    public static class Render2DOutlineStageExtensions
    {
        public static RenderPipeline Render2DOutline(this RenderPipeline pipeline, OutlineSystem outlineSystem)
        {
            var stage = new Render2DOutlineStage(outlineSystem, pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
