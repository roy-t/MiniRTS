using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Rendering
{
    [System]
    public sealed class ClearGBufferSystem : ISystem
    {
        private static readonly Color NeutralDiffuse = Color.Transparent;
        private static readonly Color NeutralDepth = Color.Transparent;
        private static readonly Color NeutralNormal = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        private static readonly Color NeutralLight = Color.Transparent;
        private static readonly Color NeutralCombine = new Color(1.0f, 0.0f, 1.0f, 1.0f);
        private static readonly Color NeutralPostProcess = new Color(1.0f, 0.0f, 1.0f, 1.0f);

        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;

        public ClearGBufferSystem(GraphicsDevice graphicsDevice, FrameService frameService)
        {
            this.Device = graphicsDevice;
            this.FrameService = frameService;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public void Process()
        {
            var renderTargetSet = this.FrameService.RenderTargetSet;
            this.ClearRenderTarget(renderTargetSet.Diffuse, NeutralDiffuse);
            this.ClearRenderTarget(renderTargetSet.Depth, NeutralDepth);
            this.ClearRenderTarget(renderTargetSet.Normal, NeutralNormal);
            this.ClearRenderTarget(renderTargetSet.Light, NeutralLight);
            this.ClearRenderTarget(renderTargetSet.Combine, NeutralCombine);
            this.ClearRenderTarget(renderTargetSet.PostProcess, NeutralPostProcess);
        }

        private void ClearRenderTarget(RenderTarget2D renderTarget, Color clearColor)
        {
            this.Device.SetRenderTarget(renderTarget);
            this.Device.Clear(clearColor);
        }

    }
}
