using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.PostProcess
{
    [System]
    public sealed class BlurSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly BlurEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        public BlurSystem(GraphicsDevice device, FullScreenTriangle fullScreenTriangle, BlurEffect effect, FrameService frameService)
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

            this.Device.SetRenderTarget(this.FrameService.PBuffer.PostProcess);
        }

        public void Process()
        {
            this.Effect.Diffuse = this.FrameService.PBuffer.Combine;
            this.Effect.SampleRadius = 0.008f;
            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
