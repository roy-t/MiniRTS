using MiniEngine.Pipeline.Models.Systems;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class RenderModelsStage : IPipelineStage<RenderPipelineInput>
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

        public void Execute(RenderPipelineInput input)
        {
            var modelBatchList = this.ModelSystem.ComputeBatches(input.Camera, input.Skybox);

            this.Input.Update(input.Camera, modelBatchList.OpaqueBatch, input.GBuffer, input.Pass);
            this.ModelPipeline.Execute(this.Input, "models");

            for (var i = 0; i < modelBatchList.TransparentBatches.Count; i++)
            {
                var batch = modelBatchList.TransparentBatches[i];
                this.Input.Update(input.Camera, batch, input.GBuffer, new Pass(PassType.Transparent, i));
                this.ModelPipeline.Execute(this.Input, $"transparent_models_{i}");
            }
        }
    }
}