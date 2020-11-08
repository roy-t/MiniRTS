using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.PostProcess
{
    [System]
    public sealed class ToneMapSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly TonemapEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        public ToneMapSystem(GraphicsDevice device, FullScreenTriangle fullScreenTriangle, TonemapEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Effect = effect;
            this.FullScreenTriangle = fullScreenTriangle;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.PBuffer.ToneMap);
        }

        public void Process()
        {
            this.Effect.Color = this.FrameService.LBuffer.Light;
            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
