using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Particles
{
    [System]
    public partial class ParticleSystem : ISystem, IParticleRendererUser
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly ParticleRenderer ParticleRenderer;
        private readonly ParticleEffect Effect;

        public ParticleSystem(GraphicsDevice device, FrameService frameService, ParticleRenderer particleRenderer, ParticleEffect effect)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.ParticleRenderer = particleRenderer;
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
        public void Process()
        {
            var camera = this.FrameService.CameraComponent.Camera;
            this.Effect.View = camera.View;
            this.ParticleRenderer.Draw(camera.ViewProjection, this);
        }

        public void ApplyEffect(Matrix worldViewProjection, ParticleEmitter emitter)
        {
            this.Effect.WorldViewProjection = worldViewProjection;
            this.Effect.Metalicness = emitter.Metalicness;
            this.Effect.Roughness = emitter.Roughness;
            this.Effect.Data = emitter.Data;

            this.Effect.Apply();
        }
    }
}
