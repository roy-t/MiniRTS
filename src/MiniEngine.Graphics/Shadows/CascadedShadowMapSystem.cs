using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Particles;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Shadows
{
    [System]
    public partial class CascadedShadowMapSystem : ISystem, IGeometryRendererUser<Matrix>, IParticleRendererUser
    {
        private readonly GraphicsDevice Device;
        private readonly GeometryRenderer Geometry;
        private readonly ParticleRenderer Particles;
        private readonly FrameService FrameService;
        private readonly ShadowMapEffect GeometryEffect;
        private readonly ParticleShadowMapEffect ParticleEffect;

        private readonly Frustum Frustum;
        private readonly RasterizerState RasterizerState;

        public CascadedShadowMapSystem(GraphicsDevice device, GeometryRenderer geometry, ParticleRenderer particles, FrameService frameService, ShadowMapEffect geometryEffect, ParticleShadowMapEffect particleEffect)
        {
            this.Device = device;
            this.Geometry = geometry;
            this.Particles = particles;
            this.FrameService = frameService;
            this.GeometryEffect = geometryEffect;
            this.ParticleEffect = particleEffect;
            this.Frustum = new Frustum();

            this.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = false
            };
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = this.RasterizerState;
        }

        [ProcessAll]
        public void Process(CascadedShadowMapComponent shadowMap, CameraComponent camera)
        {
            var view = this.FrameService.CameraComponent.Camera;
            var shadowCamera = camera.Camera;

            var surfaceToLight = -shadowCamera.Forward;

            this.Frustum.TransformToCameraFrustumInWorldSpace(view);
            shadowMap.GlobalShadowMatrix = CreateGlobalShadowMatrix(surfaceToLight, this.Frustum);

            for (var i = 0; i < shadowMap.Cascades.Length; i++)
            {
                this.Frustum.TransformToCameraFrustumInWorldSpace(view);

                var nearZ = i == 0 ? 0.0f : shadowMap.Cascades[i - 1];
                var farZ = shadowMap.Cascades[i];
                this.Frustum.Slice(nearZ, farZ);

                var viewProjection = ComputeViewProjectionMatrixForSlice(surfaceToLight, this.Frustum, shadowMap.Resolution);
                var shadowMatrix = CreateSliceShadowMatrix(viewProjection);

                var clipDistance = view.FarPlane - view.NearPlane;
                shadowMap.Splits[i] = view.NearPlane + (farZ * clipDistance);

                var nearCorner = TransformCorner(Vector3.Zero, shadowMatrix, shadowMap.GlobalShadowMatrix);
                var farCorner = TransformCorner(Vector3.One, shadowMatrix, shadowMap.GlobalShadowMatrix);

                shadowMap.Offsets[i] = new Vector4(-nearCorner, 0.0f);
                shadowMap.Scales[i] = new Vector4(Vector3.One / (farCorner - nearCorner), 1.0f);

                this.RenderShadowMap(shadowMap.DepthMapArray, i, viewProjection, camera.InView);
            }
        }

        private void RenderShadowMap(RenderTarget2D shadowMap, int index, Matrix viewProjection, IReadOnlyList<Pose> inView)
        {
            this.Device.SetRenderTarget(shadowMap, index);
            this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

            this.Geometry.Draw(inView, this, viewProjection);
            this.Particles.Draw(viewProjection, this);
        }

        public void ApplyEffect(Matrix worldViewProjection, ParticleEmitterComponent emitter)
        {
            this.ParticleEffect.WorldViewProjection = worldViewProjection;
            this.ParticleEffect.Data = emitter.Position.ReadTarget;
            this.ParticleEffect.Apply();
        }

        public void SetEffectParameters(Material material, Matrix transform, Matrix viewProjection)
            => this.GeometryEffect.WorldViewProjection = transform * viewProjection;

        public void ApplyEffect(GeometryTechnique technique)
            => this.GeometryEffect.Apply(technique);


        private static readonly Matrix TexScaleTransform = Matrix.CreateScale(0.5f, -0.5f, 1.0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);

        private static Matrix CreateGlobalShadowMatrix(Vector3 surfaceToLight, Frustum frustum)
        {
            var frustumCenter = frustum.ComputeCenter();
            var shadowCameraPos = frustumCenter + (surfaceToLight * -0.5f);

            var view = Matrix.CreateLookAt(shadowCameraPos, frustumCenter, Vector3.Up);
            var projection = Matrix.CreateOrthographicOffCenter(-0.5f, 0.5f, -0.5f, 0.5f, 0.0f, 1.0f);

            return view * projection * TexScaleTransform;
        }

        private static Matrix ComputeViewProjectionMatrixForSlice(Vector3 surfaceToLight, Frustum frustum, float resolution)
        {
            var bounds = frustum.ComputeBounds();
            var radius = (float)Math.Ceiling(bounds.Radius);

            var position = bounds.Center + (surfaceToLight * radius);

            var view = Matrix.CreateLookAt(position, bounds.Center, Vector3.Up);
            var projection = Matrix.CreateOrthographicOffCenter(
                -radius,
                radius,
                -radius,
                radius,
                0.0f,
                radius * 2);

            var origin = Vector3.Transform(Vector3.Zero, view * projection) * (resolution / 2.0f);

            var roundedOrigin = Round(origin);
            var roundOffset = (roundedOrigin - origin) * (2.0f / resolution);

            projection.M41 += roundOffset.X;
            projection.M42 += roundOffset.Y;

            return view * projection;
        }

        private static Matrix CreateSliceShadowMatrix(Matrix sliceViewProjection)
            => sliceViewProjection * TexScaleTransform;

        private static Vector3 Round(Vector3 value)
            => new Vector3(MathF.Round(value.X), MathF.Round(value.Y), MathF.Round(value.Z));

        private static Vector3 TransformCorner(Vector3 corner, Matrix shadowMatrix, Matrix globalShadowMatrix)
        {
            var inv = Matrix.Invert(shadowMatrix);
            var v = ScaleToVector3(Vector4.Transform(corner, inv));
            return ScaleToVector3(Vector4.Transform(v, globalShadowMatrix));
        }

        private static Vector3 ScaleToVector3(Vector4 value) => new Vector3(value.X, value.Y, value.Z) / value.W;
    }
}
