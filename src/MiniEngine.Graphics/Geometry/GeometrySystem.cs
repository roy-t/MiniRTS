using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Geometry
{
    [System]
    public partial class GeometrySystem : ISystem
    {
        public const int MaxInstances = 1024;

        private readonly GraphicsDevice Device;
        private readonly IComponentContainer<InstancingComponent> Instances;
        private readonly FrameService FrameService;
        private readonly GeometryEffect Effect;

        private readonly VertexBuffer InstanceBuffer;

        public GeometrySystem(GraphicsDevice device, IComponentContainer<InstancingComponent> instances, GeometryEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.Instances = instances;
            this.FrameService = frameService;
            this.Effect = effect;
            this.InstanceBuffer = new VertexBuffer(device, InstancingVertex.Declaration, MaxInstances, BufferUsage.WriteOnly);
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.AnisotropicWrap;
            this.Device.SamplerStates[1] = SamplerState.AnisotropicWrap;
            this.Device.SamplerStates[2] = SamplerState.AnisotropicWrap;
            this.Device.SamplerStates[3] = SamplerState.AnisotropicWrap;
            this.Device.SamplerStates[4] = SamplerState.AnisotropicWrap;

            this.Device.SetRenderTargets(
                this.FrameService.GBuffer.Albedo,
                this.FrameService.GBuffer.Material,
                this.FrameService.GBuffer.Depth,
                this.FrameService.GBuffer.Normal);
        }

        [Process]
        public void ProcessVisibleGeometry()
        {
            var inView = this.FrameService.CamereComponent.InView;
            for (var i = 0; i < inView.Count; i++)
            {
                var pose = inView[i];
                for (var j = 0; j < pose.Model.Meshes.Count; j++)
                {
                    var mesh = pose.Model.Meshes[j];
                    this.SetEffectParameters(this.FrameService.CamereComponent.Camera, mesh.Material, mesh.Offset * pose.Transform);

                    if (this.Instances.Contains(pose.Entity))
                    {
                        var instances = this.Instances.Get(pose.Entity);
                        this.DrawIndexed(mesh.Geometry, instances);
                    }
                    else
                    {
                        this.Draw(mesh.Geometry);
                    }
                }
            }
        }

        private void DrawIndexed(GeometryData geometry, InstancingComponent instances)
        {
            this.InstanceBuffer.SetData(instances.VertexData);

            this.Device.SetVertexBuffers(new VertexBufferBinding(geometry.VertexBuffer), new VertexBufferBinding(this.InstanceBuffer, 0, 1));
            this.Device.Indices = geometry.IndexBuffer;

            this.Effect.Apply(GeometryTechnique.Instanced);

            this.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives, instances.Instances);
        }

        private void Draw(GeometryData geometry)
        {
            this.Device.SetVertexBuffer(geometry.VertexBuffer);
            this.Device.Indices = geometry.IndexBuffer;

            this.Effect.Apply(GeometryTechnique.Default);
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives);
        }

        private void SetEffectParameters(ICamera camera, Material material, Matrix transform)
        {
            this.Effect.CameraPosition = camera.Position;
            this.Effect.World = transform;
            this.Effect.WorldViewProjection = transform * camera.ViewProjection;
            this.Effect.Albedo = material.Albedo;
            this.Effect.Normal = material.Normal;
            this.Effect.Metalicness = material.Metalicness;
            this.Effect.Roughness = material.Roughness;
            this.Effect.AmbientOcclusion = material.AmbientOcclusion;


        }
    }
}
