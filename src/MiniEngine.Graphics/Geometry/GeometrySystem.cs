using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Geometry
{
    [System]
    public partial class GeometrySystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly GeometryEffect Effect;

        public GeometrySystem(GraphicsDevice device, GeometryEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Effect = effect;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.AnisotropicClamp;
            this.Device.SamplerStates[1] = SamplerState.AnisotropicClamp;

            this.Device.SetRenderTargets(
                this.FrameService.GBuffer.Diffuse,
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
                this.Draw(this.FrameService.CamereComponent.Camera, pose.Geometry, pose.Material, pose.Transform);
            }
        }

        private void Draw(ICamera camera, GeometryData geometry, Material material, Matrix transform)
        {
            this.Effect.CameraPosition = camera.Position;
            this.Effect.World = transform;
            this.Effect.WorldViewProjection = transform * camera.ViewProjection;
            this.Effect.Diffuse = material.Diffuse;
            this.Effect.Normal = material.Normal;
            this.Effect.Metalicness = material.Metalicness;
            this.Effect.Roughness = material.Roughness;
            this.Effect.AmbientOcclusion = material.AmbientOcclusion;
            this.Effect.Apply();

            this.Device.SetVertexBuffer(geometry.VertexBuffer, 0);
            this.Device.Indices = geometry.IndexBuffer;
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives);
        }
    }
}
