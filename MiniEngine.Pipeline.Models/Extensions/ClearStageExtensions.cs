using MiniEngine.Pipeline.Models.Stages;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class ClearStageExtensions
    {
        public static ModelPipeline ClearModelRenderTargets(this ModelPipeline pipeline)
        {
            var stage = new ClearStage(pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }     
    }
}
