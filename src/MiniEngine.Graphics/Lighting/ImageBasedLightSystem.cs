using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Generated;
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
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly ImageBasedLightEffect Effect;
        private readonly Texture2D BrdfLutTexture;
        private readonly FrameService FrameService;

        public ImageBasedLightSystem(GraphicsDevice device, BrdfLutGenerator brdfLutGenerator, PostProcessTriangle postProcessTriangle, ImageBasedLightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.PostProcessTriangle = postProcessTriangle;
            this.FrameService = frameService;
            this.Effect = effect;

            this.BrdfLutTexture = brdfLutGenerator.Generate();
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        [Process]
        public void Process()
        {
            this.Effect.CameraPosition = this.FrameService.CameraComponent.Camera.Position;
            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.CameraComponent.Camera.ViewProjection);

            this.Effect.Albedo = this.FrameService.GBuffer.Albedo;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.GBufferSampler = SamplerState.LinearClamp;

            this.Effect.Irradiance = this.FrameService.Skybox.Irradiance;
            this.Effect.Environment = this.FrameService.Skybox.Environment;
            this.Effect.BrdfLut = this.BrdfLutTexture;
            this.Effect.TextureSampler = SamplerState.LinearClamp;

            this.Effect.MaxReflectionLod = this.FrameService.Skybox.Environment.LevelCount;

            this.Effect.AmbientLightFactor = this.FrameService.Skybox.AmbientLightFactor;

            this.Effect.Apply();

            this.PostProcessTriangle.Render(this.Device);
        }
    }
}
