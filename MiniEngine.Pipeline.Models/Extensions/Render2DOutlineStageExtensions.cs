using MiniEngine.Pipeline.Models.Stages;
using MiniEngine.Pipeline.Models.Systems;

namespace MiniEngine.Pipeline.Models.Extensions
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
