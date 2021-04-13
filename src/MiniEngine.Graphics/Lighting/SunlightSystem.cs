using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Lighting
{
    [System]
    public partial class SunlightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly SunlightEffect Effect;
        private readonly PostProcessTriangle PostProcessTriangle;

        public SunlightSystem(GraphicsDevice device, PostProcessTriangle postProcessTriangle, SunlightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.PostProcessTriangle = postProcessTriangle;
            this.Effect = effect;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        [ProcessAll]
        public void Process(SunlightComponent sunlight, CascadedShadowMapComponent shadowMap, CameraComponent shadowMapCamera)
        {
            this.Effect.CameraPosition = this.FrameService.CameraComponent.Camera.Position;

            this.Effect.Albedo = this.FrameService.GBuffer.Albedo;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.GBufferSampler = SamplerState.LinearClamp;

            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.CameraComponent.Camera.ViewProjection);

            this.Effect.SurfaceToLight = -shadowMapCamera.Camera.Forward;
            this.Effect.Color = sunlight.Color.ToVector4();
            this.Effect.Strength = sunlight.Strength;

            this.Effect.ShadowMap = shadowMap.DepthMapArray;
            this.Effect.ShadowSampler = ShadowMapSampler.State;

            this.Effect.ShadowMatrix = shadowMap.GlobalShadowMatrix;
            this.Effect.Splits = shadowMap.Splits;
            this.Effect.Offsets = shadowMap.Offsets;
            this.Effect.Scales = shadowMap.Scales;

            this.Effect.ApplySunlightTechnique();
            this.PostProcessTriangle.Render(this.Device);
        }
    }
}
