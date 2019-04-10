using MiniEngine.Pipeline.Models;

namespace MiniEngine.Pipeline.Projectors.Stages
{
    public sealed class RenderProjectorsStage : IPipelineStage<ModelPipelineInput>
    {
        private readonly ProjectorPipeline ProjectorPipeline;
        private readonly ProjectorPipelineInput Input;

        public RenderProjectorsStage(ProjectorPipeline projectorPipeline)
        {
            this.ProjectorPipeline = projectorPipeline;
            this.Input = new ProjectorPipelineInput();
        }

        public void Execute(ModelPipelineInput input)
        {
            this.Input.Update(input.GBuffer, input.Camera, input.Pass);
            this.ProjectorPipeline.Execute(this.Input, "projectors");
        }
    }
}
