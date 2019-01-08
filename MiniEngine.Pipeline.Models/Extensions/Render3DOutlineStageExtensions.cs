using MiniEngine.Pipeline.Models.Stages;
using MiniEngine.Pipeline.Models.Systems;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class Render3DOutlineStageExtensions
    {
        public static RenderPipeline Render3DOutline(this RenderPipeline pipeline, OutlineSystem outlineSystem)
        {
            var stage = new Render3DOutlineStage(outlineSystem, pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
