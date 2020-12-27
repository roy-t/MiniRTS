using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    [System]
    public partial class ParticipatingMediaSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly LightPostProcessEffect Effect;
        private readonly MediaEffect MediaEffect;
        private readonly ShadowMapEffect ShadowMapEffect;
        private readonly ParticipatingMediaPostProcessEffect PostProcessEffect;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly FrameService FrameService;
        private readonly SamplerState ShadowMapSampler;
        private readonly Texture2D Noise;

        public ParticipatingMediaSystem(GraphicsDevice device, ContentManager content, LightPostProcessEffect effect, MediaEffect mediaEffect, ShadowMapEffect shadowMapEffect, ParticipatingMediaPostProcessEffect postProcessEffect, PostProcessTriangle postProcessTriangle, FrameService frameService)
        {
            this.Device = device;
            this.Effect = effect;
            this.MediaEffect = mediaEffect;
            this.ShadowMapEffect = shadowMapEffect;
            this.PostProcessEffect = postProcessEffect;
            this.PostProcessTriangle = postProcessTriangle;
            this.FrameService = frameService;

            this.Noise = content.Load<Texture2D>("Textures/BlueNoise");
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
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;
            this.Device.SamplerStates[1] = SamplerState.LinearClamp;
            this.Device.SamplerStates[2] = SamplerState.LinearClamp;
            this.Device.SamplerStates[3] = SamplerState.LinearClamp;
            this.Device.SamplerStates[4] = SamplerState.PointWrap;
        }


        [ProcessAll]
        public void Process(ParticipatingMediaComponent media, CascadedShadowMapComponent shadowMap, TransformComponent transform)
        {
            var camera = this.FrameService.CamereComponent.Camera;
            this.ComputeDensity(media, transform, camera);
            this.ComputeMedia(media, shadowMap, camera);
            this.DrawMediaToLightTarget(media);
        }

        private void DrawMediaToLightTarget(ParticipatingMediaComponent media)
        {
            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
            this.Device.BlendState = BlendState.AlphaBlend;

            this.PostProcessEffect.Media = media.ParticipatingMediaBuffer;
            this.PostProcessEffect.Color = new Color(0.1f, 0.1f, 0.1f);
            this.PostProcessEffect.Apply();

            this.PostProcessTriangle.Render(this.Device);
        }

        private void ComputeMedia(ParticipatingMediaComponent media, CascadedShadowMapComponent shadowMap, ICamera camera)
        {
            this.Device.SamplerStates[0] = this.ShadowMapSampler;

            this.Device.SetRenderTarget(media.ParticipatingMediaBuffer);
            this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            this.Effect.Noise = this.Noise;
            this.Effect.Volume = media.DensityBuffer;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.InverseViewProjection = Matrix.Invert(camera.ViewProjection);
            this.Effect.CameraPosition = camera.Position;
            this.Effect.FogColor = new Color(0.1f, 0.1f, 0.1f);
            this.Effect.Strength = 4.0f;

            this.Effect.ShadowMap = shadowMap.DepthMapArray;
            this.Effect.ShadowMatrix = shadowMap.GlobalShadowMatrix;
            this.Effect.Splits = shadowMap.Splits;
            this.Effect.Offsets = shadowMap.Offsets;
            this.Effect.Scales = shadowMap.Scales;
            this.Effect.ViewDistance = camera.FarPlane;

            this.Effect.Apply();
            this.PostProcessTriangle.Render(this.Device);
        }

        private void ComputeDensity(ParticipatingMediaComponent media, TransformComponent transform, Camera.ICamera camera)
        {
            this.ShadowMapEffect.WorldViewProjection = transform.Matrix * camera.ViewProjection;
            this.DrawDistance(RasterizerState.CullClockwise, media.VolumeBackBuffer, media.Geometry);
            this.DrawDistance(RasterizerState.CullCounterClockwise, media.VolumeFrontBuffer, media.Geometry);

            this.Device.SetRenderTarget(media.DensityBuffer);
            this.Device.Clear(ClearOptions.Target, Color.White, 1.0f, 0);

            this.MediaEffect.VolumeBack = media.VolumeBackBuffer;
            this.MediaEffect.VolumeFront = media.VolumeFrontBuffer;

            this.MediaEffect.Apply();
            this.PostProcessTriangle.Render(this.Device);
        }

        private void DrawDistance(RasterizerState rasterizerState, RenderTarget2D renderTarget, GeometryData geometry)
        {
            this.Device.SetRenderTarget(renderTarget);
            this.Device.Clear(ClearOptions.Target, Color.White, 1.0f, 0);

            this.Device.SetVertexBuffer(geometry.VertexBuffer);
            this.Device.Indices = geometry.IndexBuffer;
            this.Device.RasterizerState = rasterizerState;
            this.ShadowMapEffect.Apply(GeometryTechnique.Default);

            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives);
        }
    }
}
