using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Skybox
{
    [System]
    public sealed class SkyboxSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly SkyboxEffect Effect;

        public SkyboxSystem(GraphicsDevice device, EffectFactory effectFactory, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.FullScreenTriangle = new FullScreenTriangle();

            this.Effect = effectFactory.Construct<SkyboxEffect>();
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.GBuffer.Diffuse);
        }

        public void Process(SkyboxComponent skybox)
        {
            this.Effect.Skybox = skybox.Texture;

            // TODO: do something with camera position/direction

            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
