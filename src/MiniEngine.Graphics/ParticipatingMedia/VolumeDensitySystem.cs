using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    [System]
    public partial class VolumeDensitySystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly MediaEffect MediaEffect;
        private readonly ShadowMapEffect ShadowMapEffect;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly FrameService FrameService;

        public VolumeDensitySystem(GraphicsDevice device, MediaEffect effect, ShadowMapEffect shadowMapEffect, PostProcessTriangle postProcessTriangle, FrameService frameService)
        {
            this.Device = device;
            this.MediaEffect = effect;
            this.ShadowMapEffect = shadowMapEffect;
            this.PostProcessTriangle = postProcessTriangle;
            this.FrameService = frameService;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            // Even if there are no participating media make sure that the LBuffer
            // has the right data
            this.Device.SetRenderTarget(this.FrameService.LBuffer.ParticipatingMedia);
            this.Device.Clear(ClearOptions.Target, Color.White, 1.0f, 0);
        }


        [ProcessAll]
        public void Process(DustComponent media, TransformComponent transform)
        {
            var camera = this.FrameService.CamereComponent.Camera;

            this.ShadowMapEffect.WorldViewProjection = transform.Matrix * camera.ViewProjection;
            this.DrawDistance(RasterizerState.CullClockwise, media.VolumeBackBuffer, media.Geometry);
            this.DrawDistance(RasterizerState.CullCounterClockwise, media.VolumeFrontBuffer, media.Geometry);

            this.Device.SetRenderTarget(this.FrameService.LBuffer.ParticipatingMedia);
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
