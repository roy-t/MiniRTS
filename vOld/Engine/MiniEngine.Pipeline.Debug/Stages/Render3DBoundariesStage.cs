using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Stages
{
    public sealed class Render3DBoundariesStage : IPipelineStage<RenderPipelineInput>
    {
        private readonly BoundarySystem OutlineSystem;
        private readonly GraphicsDevice Device;

        public Render3DBoundariesStage(BoundarySystem boundarySystem, GraphicsDevice device)
        {
            this.OutlineSystem = boundarySystem;
            this.Device = device;
        }

        public void Execute(RenderPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.OutlineSystem.Render3DBounds(input.Camera, input.GBuffer);
        }
    }
}
