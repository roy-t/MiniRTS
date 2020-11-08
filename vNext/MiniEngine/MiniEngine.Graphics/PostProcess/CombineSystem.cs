using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.PostProcess
{
    [System]
    public sealed class CombineSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly CombineEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        public CombineSystem(GraphicsDevice device, FullScreenTriangle fullScreenTriangle, CombineEffect effect, FrameService frameService)
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
            this.Device.SamplerStates[1] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.PBuffer.Combine);
        }

        public void Process()
        {
            this.Effect.Light = this.FrameService.LBuffer.Light;
            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
