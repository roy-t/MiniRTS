﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
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
        private readonly PostProcessTriangle Volume;

        private readonly RasterizerState SunlightRasterizer;
        private readonly SamplerState ShadowMapSampler;

        public SunlightSystem(GraphicsDevice device, PostProcessTriangle volume, SunlightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Volume = volume;
            this.Effect = effect;

            this.SunlightRasterizer = new RasterizerState()
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
            this.Device.RasterizerState = this.SunlightRasterizer;
            this.Device.SamplerStates[0] = this.ShadowMapSampler;
            this.Device.SamplerStates[1] = SamplerState.LinearClamp;
            this.Device.SamplerStates[2] = SamplerState.LinearClamp;
            this.Device.SamplerStates[3] = SamplerState.LinearClamp;
            this.Device.SamplerStates[4] = SamplerState.LinearClamp;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        [ProcessAll]
        public void Process(SunlightComponent sunlight, CascadedShadowMapComponent shadowMap, CameraComponent shadowMapCamera)
        {
            var world = Matrix.Invert(shadowMapCamera.Camera.ViewProjection);

            this.Effect.CameraPosition = this.FrameService.CamereComponent.Camera.Position;
            this.Effect.Albedo = this.FrameService.GBuffer.Albedo;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.CamereComponent.Camera.ViewProjection);

            this.Effect.SurfaceToLight = -shadowMapCamera.Camera.Forward;
            this.Effect.Color = sunlight.Color;
            this.Effect.Strength = sunlight.Strength;

            this.Effect.ShadowMap = shadowMap.DepthMapArray;

            this.Effect.ShadowMatrix = shadowMap.GlobalShadowMatrix;
            this.Effect.Splits = shadowMap.Splits;
            this.Effect.Offsets = shadowMap.Offsets;
            this.Effect.Scales = shadowMap.Scales;

            this.Effect.Apply();
            this.Volume.Render(this.Device);

            // TODO: try with full-screen triangle instead, as the volume won't match anyways
        }
    }
}
