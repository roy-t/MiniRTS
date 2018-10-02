using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class RenderModelsStage : IPipelineStage
    {
        private readonly ModelSystem ModelSystem;
        private readonly ModelPipeline ModelPipeline;

        public RenderModelsStage(ModelSystem modelSystem, ModelPipeline modelPipeline)
        {
            this.ModelSystem = modelSystem;            
            this.ModelPipeline = modelPipeline;
        }

        public void Execute(PerspectiveCamera camera)
        {
            var renderBatchList = this.ModelSystem.ComputeBatches(camera);
            this.ModelPipeline.Execute(camera, renderBatchList);
        }
    }
}
