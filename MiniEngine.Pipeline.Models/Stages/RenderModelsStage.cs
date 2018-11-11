using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class RenderModelsStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly ModelPipeline ModelPipeline;
        private readonly ModelSystem ModelSystem;
        private readonly GBuffer GBuffer;

        public RenderModelsStage(ModelSystem modelSystem, ModelPipeline modelPipeline, GBuffer gBuffer)
        {
            this.ModelSystem = modelSystem;
            this.ModelPipeline = modelPipeline;
            this.GBuffer = gBuffer;
        }

        public void Execute(RenderPipelineStageInput input)
        {
            var modelBatchList = this.ModelSystem.ComputeBatches(input.Camera);

            this.ModelPipeline.Execute(new ModelPipelineInput(input.Camera, modelBatchList.OpaqueBatch, this.GBuffer, "opaque"));


            for(var i = 0; i < modelBatchList.TransparentBatches.Count; i++)
            {
                var batch = modelBatchList.TransparentBatches[i];
                this.ModelPipeline.Execute(new ModelPipelineInput(input.Camera, batch, this.GBuffer, $"transparent_{i}"));
            }
        }
    }
}