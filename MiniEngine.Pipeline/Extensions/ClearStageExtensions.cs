using MiniEngine.Pipeline.Stages;

namespace MiniEngine.Pipeline.Extensions
{
    public static class ClearStageExtensions
    {        
        public static RenderPipeline ClearRenderTargetSet(this RenderPipeline pipeline)
        {
            var stage = new ClearStage(pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
