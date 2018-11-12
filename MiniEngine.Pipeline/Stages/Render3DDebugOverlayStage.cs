using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Systems;

namespace MiniEngine.Pipeline.Stages
{
    public sealed class Render3DDebugOverlayStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly DebugRenderSystem DebugRenderSystem;
        private readonly GraphicsDevice Device;

        public Render3DDebugOverlayStage(GraphicsDevice device, DebugRenderSystem debugRenderSystem)
        {
            this.Device = device;
            this.DebugRenderSystem = debugRenderSystem;
        }

        public void Execute(RenderPipelineStageInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.DebugRenderSystem.Render3DOverlay(input.Camera);
        }
    }
}