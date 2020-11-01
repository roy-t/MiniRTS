using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    [System]
    public sealed class ImageBasedLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly ImageBasedLightEffect Effect;
        private readonly FrameService FrameService;

        public ImageBasedLightSystem(GraphicsDevice device, FullScreenTriangle fullScreenTriangle, EffectFactory effectFactory, FrameService frameService)
        {
            this.Device = device;
            this.FullScreenTriangle = fullScreenTriangle;
            this.FrameService = frameService;

            this.Effect = effectFactory.Construct<ImageBasedLightEffect>();
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SamplerStates[0] = SamplerState.LinearClamp;
            this.Device.SamplerStates[1] = SamplerState.LinearClamp;
            this.Device.SamplerStates[2] = SamplerState.LinearClamp;
            this.Device.SamplerStates[3] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        public void Process()
        {
            this.Effect.CameraPosition = this.FrameService.Camera.Position;
            this.Effect.Diffuse = this.FrameService.GBuffer.Diffuse;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.Irradiance = this.FrameService.Skybox.Irradiance;
            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.Camera.ViewProjection);

            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
