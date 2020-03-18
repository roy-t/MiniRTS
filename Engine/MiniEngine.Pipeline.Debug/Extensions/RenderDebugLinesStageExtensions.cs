using MiniEngine.Pipeline.Debug.Stages;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Extensions
{
    public static class RenderDebugLinesStageExtensions
    {
        public static RenderPipeline RenderDebugLines(this RenderPipeline pipeline, LineSystem lineSystem)
        {
            var stage = new RenderDebugLinesStage(lineSystem, pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
