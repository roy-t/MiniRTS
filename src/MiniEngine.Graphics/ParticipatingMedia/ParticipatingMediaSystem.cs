using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.Physics;
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
        private readonly ParticipatingMediaEffect MediaEffect;
        private readonly ParticipatingMediaPostProcessEffect PostProcessEffect;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly FrameService FrameService;
        private readonly Texture2D Noise;
        private readonly Texture2D DitherPattern;
        private readonly RasterizerState FrontRasterizerState;
        private readonly RasterizerState BackRasterizerState;
        private readonly Texture2D Albedo;

        public ParticipatingMediaSystem(GraphicsDevice device, ContentManager content, ShadowMapEffect shadowMapEffect, ParticipatingMediaEffect mediaEffect, ParticipatingMediaPostProcessEffect postProcessEffect, PostProcessTriangle postProcessTriangle, FrameService frameService)
        {
            this.Device = device;
            this.Albedo = new Texture2D(device, 1, 1);
            this.Albedo.SetData(new Color[] { Color.White });
            this.MediaEffect = mediaEffect;
            this.ShadowMapEffect = shadowMapEffect;
            this.PostProcessEffect = postProcessEffect;
            this.PostProcessTriangle = postProcessTriangle;
            this.FrameService = frameService;

            this.Noise = content.Load<Texture2D>("Textures/BlueNoise");
            this.DitherPattern = content.Load<Texture2D>("Textures/DitherPattern");

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
        }

        [ProcessAll]
        public void Process(ParticipatingMediaComponent media, TransformComponent transform)
        {
            var shadowMap = this.FrameService.ShadowMap;
            var sunlight = this.FrameService.Sunlight;

            var camera = this.FrameService.CameraComponent.Camera;
            this.RenderDensity(media, transform, camera);
            this.RenderMedia(media, shadowMap, camera);
            this.RenderMediaToLightTarget(media, sunlight);
        }

        private void RenderMediaToLightTarget(ParticipatingMediaComponent media, SunlightComponent sunlight)
        {
            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
            this.Device.BlendState = BlendState.AlphaBlend;

            this.PostProcessEffect.Media = media.ParticipatingMediaBuffer;
            this.PostProcessEffect.MediaColor = media.Color.ToVector3();
            this.PostProcessEffect.LightColor = sunlight.Color.ToVector3();
            this.PostProcessEffect.LightInfluence = media.LightInfluence;

            this.PostProcessEffect.DitherPattern = this.DitherPattern;
            this.PostProcessEffect.DitherPatternSampler = SamplerState.PointWrap;

            this.PostProcessEffect.ScreenDimensions = new Vector2(this.FrameService.LBuffer.Light.Width, this.FrameService.LBuffer.Light.Height);
            this.PostProcessEffect.Apply();

            this.PostProcessTriangle.Render(this.Device);
        }

        private void RenderMedia(ParticipatingMediaComponent media, CascadedShadowMapComponent shadowMap, PerspectiveCamera camera)
        {
            this.Device.SetRenderTarget(media.ParticipatingMediaBuffer);
            this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            this.MediaEffect.Noise = this.Noise;
            this.MediaEffect.NoiseSampler = SamplerState.PointWrap;

            this.MediaEffect.VolumeBack = media.VolumeBackBuffer;
            this.MediaEffect.VolumeFront = media.VolumeFrontBuffer;
            this.MediaEffect.VolumeSampler = SamplerState.LinearClamp;

            this.MediaEffect.Depth = this.FrameService.GBuffer.Depth;
            this.MediaEffect.InverseViewProjection = Matrix.Invert(camera.ViewProjection);
            this.MediaEffect.CameraPosition = camera.Position;
            this.MediaEffect.Strength = media.Strength;

            this.MediaEffect.ShadowMap = shadowMap.DepthMapArray;
            this.MediaEffect.ShadowSampler = ShadowMapSampler.State;

            this.MediaEffect.ShadowMatrix = shadowMap.GlobalShadowMatrix;
            this.MediaEffect.Splits = shadowMap.Splits;
            this.MediaEffect.Offsets = shadowMap.Offsets;
            this.MediaEffect.Scales = shadowMap.Scales;

            this.MediaEffect.ViewDistance = camera.FarPlane;
            this.MediaEffect.MinLight = 0.1f;

            this.MediaEffect.Apply();
            this.PostProcessTriangle.Render(this.Device);
        }

        private void RenderDensity(ParticipatingMediaComponent media, TransformComponent transform, PerspectiveCamera camera)
        {
            this.ShadowMapEffect.WorldViewProjection = transform.Matrix * camera.ViewProjection;
            this.ShadowMapEffect.Albedo = this.Albedo;
            this.ShadowMapEffect.MaskSampler = SamplerState.AnisotropicWrap;

            this.RenderDistance(this.BackRasterizerState, media.VolumeBackBuffer, media.Geometry);
            this.RenderDistance(this.FrontRasterizerState, media.VolumeFrontBuffer, media.Geometry);
        }

        private void RenderDistance(RasterizerState rasterizerState, RenderTarget2D renderTarget, GeometryData geometry)
        {
            this.Device.SetRenderTarget(renderTarget);
            this.Device.Clear(ClearOptions.Target, Color.Black, 0.0f, 0);

            this.Device.SetVertexBuffer(geometry.VertexBuffer);
            this.Device.Indices = geometry.IndexBuffer;
            this.Device.RasterizerState = rasterizerState;
            this.ShadowMapEffect.ApplyShadowMapTechnique();

            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives);
        }
    }
}
