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
        public void Process(ParticleEmitterComponent emitter, TransformComponent transform)
        {
            emitter.Update(this.FrameService.Elapsed, transform.Matrix);
            if (emitter.Count > 0)
            {
                var camera = this.FrameService.CameraComponent.Camera;

                this.Effect.Texture = emitter.Texture;
                this.Effect.WorldViewProjection = camera.ViewProjection;

                this.Effect.Apply();
                this.Quad.RenderInstanced(this.Device, emitter.InstanceBuffer, emitter.Count);
            }
        }
    }
}
