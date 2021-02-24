using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Volumes
{
    [System]
    public partial class VolumeSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly VolumeEffect Effect;
        private readonly FrameService FrameService;
        private readonly GeometryData Container;

        public VolumeSystem(GraphicsDevice device, VolumeEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.Effect = effect;
            this.FrameService = frameService;
            this.Container = CubeGenerator.Generate(device); // TODO: dispose?
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise; // Reverse cull if inside?
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

        [ProcessAll]
        public void Process(VolumeComponent volume, TransformComponent transform)
        {
            var camera = this.FrameService.CameraComponent.Camera;
            var material = volume.Material;

            this.Effect.CameraPosition = camera.Forward; // TODO: HACK HACK HACK 
            this.Effect.World = transform.Matrix;
            this.Effect.WorldViewProjection = transform.Matrix * camera.ViewProjection;
            this.Effect.Albedo = material.Albedo;
            this.Effect.Normal = material.Normal;
            this.Effect.Metalicness = material.Metalicness;
            this.Effect.Roughness = material.Roughness;
            this.Effect.AmbientOcclusion = material.AmbientOcclusion;

            this.Device.SetVertexBuffer(this.Container.VertexBuffer);
            this.Device.Indices = this.Container.IndexBuffer;

            this.Effect.Apply();

            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.Container.Primitives);
        }
    }
}
