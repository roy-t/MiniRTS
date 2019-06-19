using MiniEngine.Pipeline.Models.Stages;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class ClearStageExtensions
    {
        public static ModelPipeline ClearModelRenderTargets(this ModelPipeline pipeline)
        {
            pipeline.Add(new ClearStage());
            return pipeline;
        }
    }
}
