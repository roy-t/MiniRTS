using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Stages
{
    public sealed class Render3DBoundariesStage : IPipelineStage<RenderPipelineInput>
    {
        private readonly BoundarySystem OutlineSystem;
        private readonly GraphicsDevice Device;

        public Render3DBoundariesStage(BoundarySystem outlineSystem, GraphicsDevice device)
        {
            this.OutlineSystem = outlineSystem;
            this.Device = device;
        }

        public void Execute(RenderPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.OutlineSystem.Render3DOverlay(input.Camera);
        }
    }
}
