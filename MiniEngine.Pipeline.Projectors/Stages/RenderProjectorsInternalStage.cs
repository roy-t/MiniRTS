using MiniEngine.Pipeline.Projectors.Systems;

namespace MiniEngine.Pipeline.Projectors.Stages
{
    public sealed class RenderProjectorsInternalStage : IPipelineStage<ProjectorPipelineInput>
    {
        private readonly ProjectorSystem ProjectorSystem;

        public RenderProjectorsInternalStage(ProjectorSystem projectorSystem)
        {
            this.ProjectorSystem = projectorSystem;
        }

        public void Execute(ProjectorPipelineInput input) => this.ProjectorSystem.RenderProjectors(input.Camera, input.GBuffer);
    }
}
