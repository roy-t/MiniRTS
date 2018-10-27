using MiniEngine.Pipeline.Models.Stages;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class RenderModelBatchStageExtensions
    {
        public static ModelPipeline RenderModelBatch(this ModelPipeline pipeline, GBuffer gBuffer)
        {
            var stage = new RenderModelBatchStage(pipeline.Device, gBuffer);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}