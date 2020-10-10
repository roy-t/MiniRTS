using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
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

        public BlurSystem(GraphicsDevice device, EffectFactory effectFactory, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Effect = effectFactory.Construct<BlurEffect>();
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.RenderTargetSet.PostProcess);
        }

        public void Process()
        {
            var renderTargets = this.FrameService.RenderTargetSet;

            this.Effect.Diffuse = renderTargets.Combine;
            this.Effect.SampleRadius = 0.008f;
            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
