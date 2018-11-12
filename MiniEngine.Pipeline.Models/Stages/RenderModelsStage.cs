using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class RenderModelsStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly ModelPipeline ModelPipeline;
        private readonly ModelSystem ModelSystem;

        private readonly ModelPipelineInput Input;

        public RenderModelsStage(ModelSystem modelSystem, ModelPipeline modelPipeline)
        {
            this.ModelSystem = modelSystem;
            this.ModelPipeline = modelPipeline;

            this.Input = new ModelPipelineInput();
        }

        public void Execute(RenderPipelineStageInput input)
        {
            var modelBatchList = this.ModelSystem.ComputeBatches(input.Camera);

            this.Input.Update(input.Camera, modelBatchList.OpaqueBatch, input.GBuffer, "opaque");
            this.ModelPipeline.Execute(this.Input);

            for(var i = 0; i < modelBatchList.TransparentBatches.Count; i++)
            {
                var batch = modelBatchList.TransparentBatches[i];
                this.Input.Update(input.Camera, batch, input.GBuffer, $"transparent_{i}");
                this.ModelPipeline.Execute(this.Input);
            }
        }
    }
}