using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Rendering
{
    [System]
    public partial class ClearBuffersSystem : ISystem
    {
        private static readonly Color NeutralAlbedo = Color.Transparent;
        private static readonly Color NeutralMaterial = Color.Transparent;
        private static readonly Color NeutralDepth = Color.White;
        private static readonly Color NeutralNormal = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        private static readonly Color NeutralLight = Color.Transparent;
        private static readonly Color NeutralToneMap = Color.Transparent;
        private static readonly Color NeutralWeightsColor = new Color(1.0f, 0, 0, 0);

        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;

        public ClearBuffersSystem(GraphicsDevice graphicsDevice, FrameService frameService)
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

        [Process]
        public void Process()
        {
            var gBuffer = this.FrameService.GBuffer;
            this.ClearRenderTarget(gBuffer.Albedo, NeutralAlbedo);
            this.ClearRenderTarget(gBuffer.Material, NeutralMaterial);
            this.ClearRenderTarget(gBuffer.Depth, NeutralDepth);
            this.ClearRenderTarget(gBuffer.Normal, NeutralNormal);

            var lBuffer = this.FrameService.LBuffer;
            this.ClearRenderTarget(lBuffer.Light, NeutralLight);

            var pBuffer = this.FrameService.PBuffer;
            this.ClearRenderTarget(pBuffer.ToneMap, NeutralToneMap);
        }

        private void ClearRenderTarget(RenderTarget2D renderTarget, Color clearColor)
        {
            this.Device.SetRenderTarget(renderTarget);
            this.Device.Clear(clearColor);
        }
    }
}
