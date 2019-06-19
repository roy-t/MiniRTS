using MiniEngine.Pipeline.Models.Stages;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class RenderModelBatchStageExtensions
    {
        public static ModelPipeline RenderModelBatch(this ModelPipeline pipeline)
        {
            var stage = new RenderModelBatchStage(pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}