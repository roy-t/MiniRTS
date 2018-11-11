using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Systems;

namespace MiniEngine.Pipeline.Stages
{
    public sealed class Render2DDebugOverlayStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly DebugRenderSystem DebugRenderSystem;
        private readonly RenderTarget2D DestinationTarget;
        private readonly GraphicsDevice Device;

        public Render2DDebugOverlayStage(
            GraphicsDevice device,
            DebugRenderSystem debugRenderSystem,
            RenderTarget2D destinationTarget)
        {
            this.Device = device;
            this.DebugRenderSystem = debugRenderSystem;
            this.DestinationTarget = destinationTarget;
        }

        public void Execute(RenderPipelineStageInput input)
        {
            this.Device.SetRenderTarget(this.DestinationTarget);
            this.DebugRenderSystem.Render2DOverlay(input.Camera);
        }
    }
}