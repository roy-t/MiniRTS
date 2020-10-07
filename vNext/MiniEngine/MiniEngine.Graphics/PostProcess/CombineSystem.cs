using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
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

        public CombineSystem(GraphicsDevice device, EffectFactory effectFactory, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Effect = effectFactory.Construct<CombineEffect>();
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.AnisotropicClamp;
            this.Device.SamplerStates[1] = SamplerState.AnisotropicClamp;
            this.Device.SamplerStates[2] = SamplerState.PointClamp;

            this.Device.SetRenderTarget(this.FrameService.RenderTargetSet.Combine);
        }

        public void Process()
        {
            var renderTargets = this.FrameService.RenderTargetSet;

            this.Effect.Diffuse = renderTargets.Diffuse;
            //this.Effect.Normal = renderTargets.Normal;
            //this.Effect.Depth = renderTargets.Depth;

            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
