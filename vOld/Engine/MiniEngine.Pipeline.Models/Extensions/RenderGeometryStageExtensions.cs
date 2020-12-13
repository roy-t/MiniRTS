using MiniEngine.Pipeline.Models.Stages;
using MiniEngine.Pipeline.Models.Systems;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class RenderGeometryStageExtensions
    {
        public static ModelPipeline RenderGeometry(this ModelPipeline pipeline, GeometrySystem geometrySystem)
        {
            var stage = new RenderGeometryStage(pipeline.Device, geometrySystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}