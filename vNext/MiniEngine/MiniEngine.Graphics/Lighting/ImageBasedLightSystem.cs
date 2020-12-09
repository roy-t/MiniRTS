using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Utilities;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Lighting
{
    [System]
    public partial class ImageBasedLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly ImageBasedLightEffect Effect;
        private readonly Texture2D BrdfLutTexture;
        private readonly FrameService FrameService;

        public ImageBasedLightSystem(GraphicsDevice device, BrdfLutGenerator brdfLutGenerator, FullScreenTriangle fullScreenTriangle, ImageBasedLightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FullScreenTriangle = fullScreenTriangle;
            this.FrameService = frameService;
            this.Effect = effect;

            this.BrdfLutTexture = brdfLutGenerator.Generate();
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
            this.Device.SamplerStates[4] = SamplerState.LinearClamp;
            this.Device.SamplerStates[5] = SamplerState.LinearClamp;
            this.Device.SamplerStates[6] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        [Process]
        public void Process()
        {
            this.Effect.CameraPosition = this.FrameService.CamereComponent.Camera.Position;
            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.CamereComponent.Camera.ViewProjection);

            this.Effect.Diffuse = this.FrameService.GBuffer.Diffuse;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.Irradiance = this.FrameService.Skybox.Irradiance;
            this.Effect.Environment = this.FrameService.Skybox.Environment;
            this.Effect.BrdfLut = this.BrdfLutTexture;

            this.Effect.MaxReflectionLod = this.FrameService.Skybox.Environment.LevelCount;

            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}
