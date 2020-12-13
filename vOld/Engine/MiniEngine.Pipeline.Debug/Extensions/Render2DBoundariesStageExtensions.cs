using MiniEngine.Pipeline.Debug.Stages;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Extensions
{
    public static class Render2DBoundariesStageExtensions
    {
        public static RenderPipeline Render2DOutline(this RenderPipeline pipeline, BoundarySystem boundarySystem)
        {
            var stage = new Render2DBoundariesStage(boundarySystem, pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
