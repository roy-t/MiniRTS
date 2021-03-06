﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Generated;
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
        private readonly FrustumLightVolume Volume;

        private readonly RasterizerState SpotLightRasterizer;

        public SpotLightSystem(GraphicsDevice device, FrustumLightVolume volume, SpotLightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Volume = volume;
            this.Effect = effect;

            this.SpotLightRasterizer = new RasterizerState()
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                DepthClipEnable = false
            };
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = this.SpotLightRasterizer;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        [ProcessAll]
        public void Process(SpotLightComponent spotLight, ShadowMapComponent shadowMap, CameraComponent shadowMapCamera)
        {
            var world = Matrix.Invert(shadowMapCamera.Camera.ViewProjection);

            this.Effect.WorldViewProjection = world * this.FrameService.CameraComponent.Camera.ViewProjection;
            this.Effect.CameraPosition = this.FrameService.CameraComponent.Camera.Position;

            this.Effect.Albedo = this.FrameService.GBuffer.Albedo;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.GBufferSampler = SamplerState.LinearClamp;

            this.Effect.InverseViewProjection = Matrix.Invert(this.FrameService.CameraComponent.Camera.ViewProjection);

            this.Effect.Position = shadowMapCamera.Camera.Position;
            this.Effect.Color = spotLight.Color.ToVector4();
            this.Effect.Strength = spotLight.Strength;

            this.Effect.ShadowMap = shadowMap.DepthMap;
            this.Effect.ShadowSampler = ShadowMapSampler.State;

            this.Effect.ShadowViewProjection = shadowMapCamera.Camera.ViewProjection;

            this.Effect.Apply();
            this.Volume.Render(this.Device);
        }
    }
}
