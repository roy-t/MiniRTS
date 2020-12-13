using MiniEngine.Pipeline.Models;
using MiniEngine.Pipeline.Projectors.Stages;
using MiniEngine.Pipeline.Projectors.Systems;

namespace MiniEngine.Pipeline.Projectors.Extensions
{
    public static class PipelineExtensions
    {
        public static ModelPipeline RenderProjectors(this ModelPipeline pipeline, ProjectorPipeline projectorPipeline)
        {
            var stage = new RenderProjectorsStage(projectorPipeline);
            pipeline.Add(stage);

            return pipeline;
        }

        public static ProjectorPipeline RenderProjectors(this ProjectorPipeline pipeline, ProjectorSystem projectorSystem)
        {
            var stage = new RenderProjectorsInternalStage(pipeline.Device, projectorSystem);
            pipeline.Add(stage);

            return pipeline;
        }
    }
}
