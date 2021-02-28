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
        private readonly Quad Quad;
        private readonly ParticleEffect Effect;

        public ParticleSystem(GraphicsDevice device, FrameService frameService, Quad quad, ParticleEffect effect)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Quad = quad;
            this.Effect = effect;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.DepthRead;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            // Set the albedo target only for the depth buffer
            this.Device.SetRenderTargets(this.FrameService.GBuffer.Albedo, this.FrameService.LBuffer.Light);
        }

        [ProcessAll]
        public void Process(ParticleFountainComponent fountain, TransformComponent transform)
        {
            var camera = this.FrameService.CameraComponent.Camera;
            fountain.Update(this.FrameService.Elapsed, transform.Matrix, camera);

            for (var i = 0; i < fountain.Emitters.Count; i++)
            {
                var emitter = fountain.Emitters[i];

                this.Effect.WorldViewProjection = camera.ViewProjection;
                this.Effect.View = camera.View;

                this.Effect.Apply();
                this.Quad.RenderInstanced(this.Device, emitter.Particles);
            }
        }
    }
}
