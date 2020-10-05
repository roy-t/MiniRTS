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
        private static readonly Color NeutralNormal = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        private static readonly Color NeutralDepth = Color.Transparent;
        private static readonly Color NeutralResolve = Color.CornflowerBlue;

        private readonly GraphicsDevice GraphicsDevice;
        private readonly FrameService FrameService;

        public ClearGBufferSystem(GraphicsDevice graphicsDevice, FrameService frameService)
        {
            this.GraphicsDevice = graphicsDevice;
            this.FrameService = frameService;
        }

        public void OnSet()
        {

        }

        public void Process()
        {
            var renderTargetSet = this.FrameService.RenderTargetSet!;
            this.ClearRenderTarget(renderTargetSet.Diffuse, NeutralDiffuse);
            this.ClearRenderTarget(renderTargetSet.Normal, NeutralNormal);
            this.ClearRenderTarget(renderTargetSet.Depth, NeutralDepth);
            this.ClearRenderTarget(renderTargetSet.Resolve, NeutralResolve);
        }


        private void ClearRenderTarget(RenderTarget2D renderTarget, Color clearColor)
        {
            this.GraphicsDevice.SetRenderTarget(renderTarget);
            this.GraphicsDevice.Clear(clearColor);
        }

    }
}
