using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class RenderModelsStage : IPipelineStage
    {
        private readonly ModelPipeline ModelPipeline;
        private readonly ModelSystem ModelSystem;

        public RenderModelsStage(ModelSystem modelSystem, ModelPipeline modelPipeline)
        {
            this.ModelSystem = modelSystem;
            this.ModelPipeline = modelPipeline;
        }

        public void Execute(PerspectiveCamera camera, Seconds seconds)
        {
            var modelBatchList = this.ModelSystem.ComputeBatches(camera);
            this.ModelPipeline.Execute(camera, modelBatchList);
        }
    }
}