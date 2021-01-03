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
        private readonly ShadowMapEffect ShadowMapEffect;
        private readonly VolumeEffect VolumeEffect;
        private readonly ParticipatingMediaEffect MediaEffect;
        private readonly ParticipatingMediaPostProcessEffect PostProcessEffect;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly FrameService FrameService;
        private readonly SamplerState ShadowMapSampler;
        private readonly Texture2D Noise;
        private readonly Texture2D DitherPattern;
        private readonly RasterizerState FrontRasterizerState;
        private readonly RasterizerState BackRasterizerState;

        public ParticipatingMediaSystem(GraphicsDevice device, ContentManager content, ShadowMapEffect shadowMapEffect, VolumeEffect volumeEffect, ParticipatingMediaEffect mediaEffect, ParticipatingMediaPostProcessEffect postProcessEffect, PostProcessTriangle postProcessTriangle, FrameService frameService)
        {
            this.Device = device;
            this.MediaEffect = mediaEffect;
            this.VolumeEffect = volumeEffect;
            this.ShadowMapEffect = shadowMapEffect;
            this.PostProcessEffect = postProcessEffect;
            this.PostProcessTriangle = postProcessTriangle;
            this.FrameService = frameService;

            this.Noise = content.Load<Texture2D>("Textures/BlueNoise");
            this.DitherPattern = content.Load<Texture2D>("Textures/DitherPattern");
            this.ShadowMapSampler = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Anisotropic,
                ComparisonFunction = CompareFunction.LessEqual,
                FilterMode = TextureFilterMode.Comparison
            };

            this.FrontRasterizerState = new RasterizerState
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                DepthClipEnable = false
            };

            this.BackRasterizerState = new RasterizerState
            {
                CullMode = CullMode.CullClockwiseFace,
                DepthClipEnable = false
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
            this.RenderDensity(media, transform, camera);
            this.RenderMedia(media, shadowMap, camera);
            this.RenderMediaToLightTarget(media);
        }

        private void RenderMediaToLightTarget(ParticipatingMediaComponent media)
        {
            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
            this.Device.BlendState = BlendState.AlphaBlend;

            this.PostProcessEffect.Media = media.ParticipatingMediaBuffer;
            this.PostProcessEffect.Color = media.Color;
            this.PostProcessEffect.DitherPattern = this.DitherPattern;
            this.PostProcessEffect.ScreenDimensions = new Vector2(this.FrameService.LBuffer.Light.Width, this.FrameService.LBuffer.Light.Height);
            this.PostProcessEffect.Apply();

            this.PostProcessTriangle.Render(this.Device);
        }

        private void RenderMedia(ParticipatingMediaComponent media, CascadedShadowMapComponent shadowMap, ICamera camera)
        {
            this.Device.SamplerStates[0] = this.ShadowMapSampler;

            this.Device.SetRenderTarget(media.ParticipatingMediaBuffer);
            this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            this.MediaEffect.Noise = this.Noise;
            this.MediaEffect.Volume = media.DensityBuffer;
            this.MediaEffect.Depth = this.FrameService.GBuffer.Depth;
            this.MediaEffect.InverseViewProjection = Matrix.Invert(camera.ViewProjection);
            this.MediaEffect.CameraPosition = camera.Position;
            this.MediaEffect.Strength = media.Strength;

            this.MediaEffect.ShadowMap = shadowMap.DepthMapArray;
            this.MediaEffect.ShadowMatrix = shadowMap.GlobalShadowMatrix;
            this.MediaEffect.Splits = shadowMap.Splits;
            this.MediaEffect.Offsets = shadowMap.Offsets;
            this.MediaEffect.Scales = shadowMap.Scales;
            this.MediaEffect.ViewDistance = camera.FarPlane;
            this.MediaEffect.MinLight = 0.1f;

            this.MediaEffect.Apply();
            this.PostProcessTriangle.Render(this.Device);
        }

        private void RenderDensity(ParticipatingMediaComponent media, TransformComponent transform, Camera.ICamera camera)
        {
            // TODO: if we're inside a very large media the front buffer and back buffer will contain distance == 1
            // which leads to a hole in the fog
            this.ShadowMapEffect.WorldViewProjection = transform.Matrix * camera.ViewProjection;
            this.RenderDistance(this.BackRasterizerState, media.VolumeBackBuffer, media.Geometry);
            this.RenderDistance(this.FrontRasterizerState, media.VolumeFrontBuffer, media.Geometry);

            this.Device.SetRenderTarget(media.DensityBuffer);
            this.Device.Clear(ClearOptions.Target, Color.White, 1.0f, 0);

            this.VolumeEffect.VolumeBack = media.VolumeBackBuffer;
            this.VolumeEffect.VolumeFront = media.VolumeFrontBuffer;

            this.VolumeEffect.Apply();
            this.PostProcessTriangle.Render(this.Device);
        }

        private void RenderDistance(RasterizerState rasterizerState, RenderTarget2D renderTarget, GeometryData geometry)
        {
            this.Device.SetRenderTarget(renderTarget);
            this.Device.Clear(ClearOptions.Target, Color.Black, 0.0f, 0);

            this.Device.SetVertexBuffer(geometry.VertexBuffer);
            this.Device.Indices = geometry.IndexBuffer;
            this.Device.RasterizerState = rasterizerState;
            this.ShadowMapEffect.Apply(GeometryTechnique.Default);

            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives);
        }
    }
}
