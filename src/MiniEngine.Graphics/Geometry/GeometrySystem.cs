using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Geometry
{
    [System]
    public partial class GeometrySystem : ISystem, IGeometryRendererUser
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly GeometryRenderer Renderer;
        private readonly GeometryEffect Effect;

        public GeometrySystem(GraphicsDevice device, GeometryRenderer renderer, GeometryEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Renderer = renderer;
            this.Effect = effect;
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
            var inView = this.FrameService.CameraComponent.InView;
            this.Renderer.Draw(inView, this);
        }

        public void SetEffectParameters(Material material, Matrix transform)
        {
            var camera = this.FrameService.CameraComponent.Camera;

            this.Effect.CameraPosition = camera.Position;
            this.Effect.World = transform;
            this.Effect.WorldViewProjection = transform * camera.ViewProjection;
            this.Effect.Albedo = material.Albedo;
            this.Effect.Normal = material.Normal;
            this.Effect.Metalicness = material.Metalicness;
            this.Effect.Roughness = material.Roughness;
            this.Effect.AmbientOcclusion = material.AmbientOcclusion;
        }

        public void ApplyEffect(GeometryTechnique technique)
            => this.Effect.Apply(technique);
    }
}
