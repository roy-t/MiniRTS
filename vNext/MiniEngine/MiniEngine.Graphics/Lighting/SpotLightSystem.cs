using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Lighting.Volumes;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Lighting
{
    [System]
    public partial class SpotLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly SpotLightEffect Effect;
        private readonly FrustumLightVolume FrustumLightVolume;

        private readonly RasterizerState SpotLightRasterizer;
        private readonly SamplerState ShadowMapSampler;

        public SpotLightSystem(GraphicsDevice device, FrustumLightVolume frustumLightVolume, SpotLightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.FrustumLightVolume = frustumLightVolume;
            this.Effect = effect;

            this.SpotLightRasterizer = new RasterizerState()
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                DepthClipEnable = false
            };

            this.ShadowMapSampler = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Anisotropic,
                ComparisonFunction = CompareFunction.LessEqual,
                FilterMode = TextureFilterMode.Comparison
            };
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = this.SpotLightRasterizer;
            this.Device.SamplerStates[0] = this.ShadowMapSampler;
            this.Device.SamplerStates[1] = SamplerState.LinearClamp;
            this.Device.SamplerStates[2] = SamplerState.LinearClamp;
            this.Device.SamplerStates[3] = SamplerState.LinearClamp;
            this.Device.SamplerStates[4] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        [ProcessAll]
        public void Process(SpotLightComponent spotLight, ShadowMapComponent shadowMap, CameraComponent shadowMapCamera)
        {
            var world = Matrix.Invert(shadowMapCamera.Camera.ViewProjection);

            this.Effect.WorldViewProjection = world * this.FrameService.CamereComponent.Camera.ViewProjection;
            this.Effect.CameraPosition = this.FrameService.CamereComponent.Camera.Position;
            this.Effect.Diffuse = this.FrameService.GBuffer.Diffuse;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.CamereComponent.Camera.ViewProjection);

            this.Effect.Position = shadowMapCamera.Camera.Position;
            this.Effect.Color = spotLight.Color;
            this.Effect.Strength = spotLight.Strength;

            this.Effect.ShadowMap = shadowMap.DepthMap;
            this.Effect.ShadowViewProjection = shadowMapCamera.Camera.ViewProjection;

            this.Effect.Apply();
            this.FrustumLightVolume.Render(this.Device);
        }
    }
}
