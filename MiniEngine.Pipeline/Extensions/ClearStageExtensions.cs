using MiniEngine.Pipeline.Stages;

namespace MiniEngine.Pipeline.Extensions
{
    public static class ClearStageExtensions
    {        
        public static RenderPipeline ClearRenderTargetSet(this RenderPipeline pipeline)
        {
            pipeline.Add(new ClearStage());
            return pipeline;
        }
    }
}
