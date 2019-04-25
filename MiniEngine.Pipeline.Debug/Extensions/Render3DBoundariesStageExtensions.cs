using MiniEngine.Pipeline.Debug.Stages;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Extensions
{
    public static class Render3DBoundariesStageExtensions
    {
        public static RenderPipeline Render3DOutline(this RenderPipeline pipeline, BoundarySystem outlineSystem)
        {
            var stage = new Render3DBoundariesStage(outlineSystem, pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
