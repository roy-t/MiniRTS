using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Particles
{
    [System]
    public partial class ParticleSystem : ISystem, IParticleRendererUser
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly ParticleRenderer ParticleRenderer;
        private readonly ParticleEffect Effect;
        private readonly SimulationEffect SimulationEffect;

        public ParticleSystem(GraphicsDevice device, FrameService frameService, PostProcessTriangle postProcessTriangle, ParticleRenderer particleRenderer, ParticleEffect effect, SimulationEffect simulationEffect)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.PostProcessTriangle = postProcessTriangle;
            this.ParticleRenderer = particleRenderer;
            this.Effect = effect;
            this.SimulationEffect = simulationEffect;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.PointClamp;
        }

        [ProcessAll]
        public void SimulateParticles(ParticleFountainComponent component)
        {
            if (component.IsEnabled)
            {
                this.SimulationEffect.Elapsed = this.FrameService.Elapsed;

                for (var i = 0; i < component.Emitters.Count; i++)
                {
                    var emitter = component.Emitters[i];
                    this.SimulationEffect.Data = emitter.FrontBuffer;

                    this.SimulationEffect.LengthScale = emitter.LengthScale;
                    this.SimulationEffect.FieldSpeed = emitter.FieldSpeed;
                    this.SimulationEffect.NoiseStrength = emitter.NoiseStrength;
                    this.SimulationEffect.ProgressionRate = emitter.ProgressionRate;
                    this.SimulationEffect.FieldMainDirection = emitter.FieldMainDirection;
                    this.SimulationEffect.SpherePosition = emitter.SpherePosition;
                    this.SimulationEffect.SphereRadius = emitter.SphereRadius;

                    this.SimulationEffect.Elapsed = this.FrameService.Elapsed;
                    this.SimulationEffect.Time = this.FrameService.Time;

                    this.SimulationEffect.Apply();

                    this.Device.SetRenderTarget(emitter.BackBuffer);
                    this.PostProcessTriangle.Render(this.Device);

                    emitter.Swap();
                }
            }
        }

        [Process]
        public void Process()
        {
            this.Device.SetRenderTargets(
               this.FrameService.GBuffer.Albedo,
               this.FrameService.GBuffer.Material,
               this.FrameService.GBuffer.Depth,
               this.FrameService.GBuffer.Normal);

            var camera = this.FrameService.CameraComponent.Camera;
            this.Effect.View = camera.View;
            this.ParticleRenderer.Draw(camera.ViewProjection, this);
        }

        public void ApplyEffect(Matrix worldViewProjection, ParticleEmitter emitter)
        {
            this.Effect.WorldViewProjection = worldViewProjection;
            this.Effect.Metalicness = emitter.Metalicness;
            this.Effect.Roughness = emitter.Roughness;
            this.Effect.Data = emitter.FrontBuffer;

            this.Effect.Apply();
        }
    }
}
