using MiniEngine.Pipeline.Debug.Stages;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Extensions
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
