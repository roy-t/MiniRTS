using MiniEngine.Pipeline.Debug.Stages;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Extensions
{
    public static class Render3DBoundariesStageExtensions
    {
        public static RenderPipeline Render3DOutline(this RenderPipeline pipeline, BoundarySystem boundarySystem)
        {
            var stage = new Render3DBoundariesStage(boundarySystem, pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
