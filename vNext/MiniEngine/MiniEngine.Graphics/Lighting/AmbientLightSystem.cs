using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    [System]
    public sealed class AmbientLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly AmbientLightEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        public AmbientLightSystem(GraphicsDevice device, EffectFactory effectFactory, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;

            this.Effect = effectFactory.Construct<AmbientLightEffect>();
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.RenderTargetSet.Light);
        }

        public void Process(AmbientLightComponent ambientLight)
        {
            //var renderTargets = this.FrameService.RenderTargetSet;

            //this.Effect.Depth = renderTargets.Depth;
            //this.Effect.Normal = renderTargets.Normal;
            this.Effect.Color = ambientLight.Color;

            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
