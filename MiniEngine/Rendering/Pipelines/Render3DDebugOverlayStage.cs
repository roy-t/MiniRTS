using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class Render3DDebugOverlayStage : IModelPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly DebugRenderSystem DebugRenderSystem;
        private readonly GBuffer GBuffer;

        public Render3DDebugOverlayStage(GraphicsDevice device, DebugRenderSystem debugRenderSystem, GBuffer gBuffer)
        {
            this.Device = device;
            this.DebugRenderSystem = debugRenderSystem;
            this.GBuffer = gBuffer;
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch batch)
        {
            this.Device.SetRenderTargets(this.GBuffer.DiffuseTarget, this.GBuffer.NormalTarget, this.GBuffer.DepthTarget);
            this.DebugRenderSystem.Render3DOverlay(camera);
        }
    }
}
