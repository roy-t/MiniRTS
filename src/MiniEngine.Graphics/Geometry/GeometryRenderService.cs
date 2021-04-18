using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.Physics;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.Graphics.Geometry
{
    [Service]
    public sealed class GeometryRenderService : IRenderService
    {
        public const int MaxInstances = 1024;

        private readonly IComponentContainer<InstancingComponent> Instances;
        private readonly IComponentContainer<GeometryComponent> Geometry;
        private readonly IComponentContainer<TransformComponent> Transforms;

        private readonly GeometryEffect GBufferEffect;
        private readonly ShadowMapEffect ShadowMapEffect;

        private readonly GraphicsDevice Device;
        private readonly VertexBuffer InstanceBuffer;

        public GeometryRenderService(
            IComponentContainer<InstancingComponent> instances,
            IComponentContainer<GeometryComponent> geometry,
            IComponentContainer<TransformComponent> transforms,
            GeometryEffect effect,
            ShadowMapEffect shadowMapEffect,
            GraphicsDevice device)
        {
            this.Instances = instances;
            this.Geometry = geometry;
            this.Transforms = transforms;
            this.GBufferEffect = effect;
            this.ShadowMapEffect = shadowMapEffect;
            this.Device = device;

            this.InstanceBuffer = new VertexBuffer(device, InstancingVertex.Declaration, MaxInstances, BufferUsage.WriteOnly);
        }

        public void DrawToShadowMap(Matrix viewProjection, Entity entity)
        {
            var geometry = this.Geometry.Get(entity);
            var transform = this.Transforms.Get(entity);

            if (this.Instances.Contains(entity))
            {
                var instances = this.Instances.Get(entity);
                this.InstanceBuffer.SetData(instances.VertexData);

                for (var i = 0; i < geometry.Geometry.Meshes.Count; i++)
                {
                    var mesh = geometry.Geometry.Meshes[i];
                    this.SetBuffers(mesh);
                    this.SetShadowMapEffectParameters(viewProjection, transform.Transform, mesh.Material);

                    this.GBufferEffect.ApplyGeometryTechnique();
                    this.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.Geometry.Primitives, instances.Instances);
                }
            }
            else
            {
                for (var i = 0; i < geometry.Geometry.Meshes.Count; i++)
                {
                    var mesh = geometry.Geometry.Meshes[i];
                    this.SetBuffers(mesh);
                    this.SetShadowMapEffectParameters(viewProjection, transform.Transform, mesh.Material);

                    this.GBufferEffect.ApplyGeometryTechnique();
                    this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.Geometry.Primitives);
                }
            }
        }

        public void DrawToGBuffer(PerspectiveCamera camera, Entity entity)
        {
            var geometry = this.Geometry.Get(entity);
            var transform = this.Transforms.Get(entity);

            if (this.Instances.Contains(entity))
            {
                var instances = this.Instances.Get(entity);
                this.InstanceBuffer.SetData(instances.VertexData);

                for (var i = 0; i < geometry.Geometry.Meshes.Count; i++)
                {
                    var mesh = geometry.Geometry.Meshes[i];
                    this.SetBuffers(mesh);
                    this.SetGBufferEffectParameters(camera, transform.Transform, mesh.Material);

                    this.GBufferEffect.ApplyGeometryTechnique();
                    this.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.Geometry.Primitives, instances.Instances);
                }
            }
            else
            {
                for (var i = 0; i < geometry.Geometry.Meshes.Count; i++)
                {
                    var mesh = geometry.Geometry.Meshes[i];
                    this.SetBuffers(mesh);
                    this.SetGBufferEffectParameters(camera, transform.Transform, mesh.Material);

                    this.GBufferEffect.ApplyGeometryTechnique();
                    this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.Geometry.Primitives);
                }
            }
        }

        private void SetBuffers(GeometryMesh mesh)
        {
            var geometryData = mesh.Geometry;
            this.Device.SetVertexBuffer(geometryData.VertexBuffer);
            this.Device.Indices = geometryData.IndexBuffer;
        }

        private void SetGBufferEffectParameters(PerspectiveCamera camera, Transform transform, Material material)
        {
            this.GBufferEffect.CameraPosition = camera.Position;
            this.GBufferEffect.World = transform.Matrix;
            this.GBufferEffect.WorldViewProjection = transform.Matrix * camera.ViewProjection;
            this.GBufferEffect.Albedo = material.Albedo;
            this.GBufferEffect.Normal = material.Normal;
            this.GBufferEffect.Metalicness = material.Metalicness;
            this.GBufferEffect.Roughness = material.Roughness;
            this.GBufferEffect.AmbientOcclusion = material.AmbientOcclusion;

            this.GBufferEffect.AnisotropicSampler = SamplerState.AnisotropicWrap;
        }

        private void SetShadowMapEffectParameters(Matrix viewProjection, Transform transform, Material material)
        {
            this.ShadowMapEffect.WorldViewProjection = transform.Matrix * viewProjection;
            this.ShadowMapEffect.Albedo = material.Albedo;
            this.ShadowMapEffect.MaskSampler = SamplerState.AnisotropicWrap;
        }
    }
}
