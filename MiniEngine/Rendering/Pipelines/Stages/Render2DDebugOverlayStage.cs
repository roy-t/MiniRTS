using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class Render2DDebugOverlayStage : IPipelineStage
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

        public void Execute(PerspectiveCamera camera, Seconds seconds)
        {
            this.Device.SetRenderTarget(this.DestinationTarget);
            this.DebugRenderSystem.Render2DOverlay(camera);
        }
    }
}