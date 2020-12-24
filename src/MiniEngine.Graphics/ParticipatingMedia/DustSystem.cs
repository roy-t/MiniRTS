using Microsoft.Xna.Framework;
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
    public partial class DustSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly MediaEffect MediaEffect;
        private readonly ShadowMapEffect ShadowMapEffect;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly RasterizerState RasterizerState;

        public DustSystem(GraphicsDevice device, MediaEffect effect, ShadowMapEffect shadowMapEffect, PostProcessTriangle postProcessTriangle)
        {
            this.Device = device;
            this.MediaEffect = effect;
            this.ShadowMapEffect = shadowMapEffect;
            this.PostProcessTriangle = postProcessTriangle;
            this.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = false
            };
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;
        }

        [ProcessAll]
        public void Process(DustComponent media, TransformComponent transform, CameraComponent camera)
        {
            this.ShadowMapEffect.WorldViewProjection = transform.Matrix * camera.Camera.ViewProjection;
            this.DrawDistance(RasterizerState.CullClockwise, media.VolumeBackBuffer, media.Geometry);
            this.DrawDistance(RasterizerState.CullCounterClockwise, media.VolumeFrontBuffer, media.Geometry);

            this.Device.SetRenderTarget(media.DensityBuffer);
            this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

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

        //public void Process(DustComponent media, TransformComponent transform, CameraComponent camera)
        //{
        //    this.Device.SetVertexBuffer(media.Geometry.VertexBuffer);
        //    this.Device.Indices = media.Geometry.IndexBuffer;
        //    this.ShadowMapEffect.WorldViewProjection = transform.Matrix * camera.Camera.ViewProjection;

        //    this.DrawDistances(RasterizerState.CullClockwise, media.VolumeBackBuffer, media.Geometry.Primitives);
        //    this.DrawDistances(RasterizerState.CullCounterClockwise, media.VolumeFrontBuffer, media.Geometry.Primitives);

        //    // Combine
        //    this.Device.BlendState = BlendState.Opaque;
        //    this.Device.SetRenderTarget(media.DensityBuffer);
        //    this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

        //    this.Effect.WorldViewProjection = Matrix.Identity;
        //    this.Effect.VolumeFront = media.VolumeFrontBuffer;
        //    this.Effect.VolumeBack = media.VolumeBackBuffer;
        //    this.Effect.Apply(MediaTechnique.Density);
        //    this.PostProcessTriangle.Render(this.Device);

        //    this.Device.SetRenderTarget(null);

        //}

        //private void DrawDistances(RasterizerState rasterizerState, RenderTarget2D target, int primitives)
        //{
        //    this.Device.SetRenderTarget(target);
        //    this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
        //    this.Device.RasterizerState = rasterizerState;

        //    this.ShadowMapEffect.Apply(GeometryTechnique.Default);
        //    this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, primitives);
        //}
    }
}
