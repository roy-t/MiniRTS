using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Particles
{
    [System]
    public partial class ParticleSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly Point Point;
        private readonly ParticleEffect Effect;

        public ParticleSystem(GraphicsDevice device, FrameService frameService, Point point, ParticleEffect effect)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Point = point;
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

        [ProcessAll]
        public void Process(ParticleFountainComponent fountain, TransformComponent transform)
        {
            var camera = this.FrameService.CameraComponent.Camera;
            for (var i = 0; i < fountain.Emitters.Count; i++)
            {
                var emitter = fountain.Emitters[i];

                this.Effect.WorldViewProjection = transform.Matrix * camera.ViewProjection;
                this.Effect.View = camera.View;
                this.Effect.Metalicness = emitter.Metalicness;
                this.Effect.Roughness = emitter.Roughness;
                this.Effect.Data = emitter.Data;

                this.Effect.Apply();
                this.Point.RenderInstanced(this.Device, emitter.Instances, emitter.Count);
            }
        }
    }
}
