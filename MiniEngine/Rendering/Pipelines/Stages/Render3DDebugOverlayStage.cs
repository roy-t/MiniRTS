using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class Render3DDebugOverlayStage : IPipelineStage
    {
        private readonly DebugRenderSystem DebugRenderSystem;
        private readonly GraphicsDevice Device;
        private readonly RenderTarget2D Target;

        public Render3DDebugOverlayStage(GraphicsDevice device, DebugRenderSystem debugRenderSystem, RenderTarget2D target)
        {
            this.Device = device;
            this.DebugRenderSystem = debugRenderSystem;
            this.Target = target;
        }

        public void Execute(PerspectiveCamera camera, Seconds seconds)
        {
            this.Device.SetRenderTarget(this.Target);
            this.DebugRenderSystem.Render3DOverlay(camera);
        }
    }
}