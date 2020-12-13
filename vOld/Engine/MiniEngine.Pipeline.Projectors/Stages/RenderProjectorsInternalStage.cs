using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Systems;

namespace MiniEngine.Pipeline.Projectors.Stages
{
    public sealed class RenderProjectorsInternalStage : IPipelineStage<ProjectorPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly ProjectorSystem ProjectorSystem;

        public RenderProjectorsInternalStage(GraphicsDevice device, ProjectorSystem projectorSystem)
        {
            this.Device = device;
            this.ProjectorSystem = projectorSystem;
        }

        public void Execute(ProjectorPipelineInput input)
        {
            // Do not render over transparent objects
            if (input.Pass.Type == PassType.Opaque)
            {
                this.Device.SetRenderTarget(input.GBuffer.DiffuseTarget);
                this.ProjectorSystem.RenderProjectors(input.Camera, input.GBuffer);
            }
        }
    }
}
