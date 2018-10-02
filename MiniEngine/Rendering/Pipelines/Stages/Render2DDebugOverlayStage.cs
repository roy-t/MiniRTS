using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class Render2DDebugOverlayStage : IPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly DebugRenderSystem DebugRenderSystem;
        private readonly RenderTarget2D DestinationTarget;

        public Render2DDebugOverlayStage(GraphicsDevice device, DebugRenderSystem debugRenderSystem, RenderTarget2D destinationTarget)
        {
            this.Device = device;
            this.DebugRenderSystem = debugRenderSystem;
            this.DestinationTarget = destinationTarget;
        }

        public void Execute(PerspectiveCamera camera)
        {
            this.Device.SetRenderTarget(this.DestinationTarget);
            this.DebugRenderSystem.Render2DOverlay(camera);
        }
    }
}
