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
        private readonly ParticleSimulationEffect SimulationEffect;

        public ParticleSystem(GraphicsDevice device, FrameService frameService, PostProcessTriangle postProcessTriangle, ParticleRenderer particleRenderer, ParticleEffect effect, ParticleSimulationEffect simulationEffect)
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
            this.Device.SamplerStates[1] = SamplerState.PointClamp;
            this.Device.SamplerStates[2] = SamplerState.PointClamp;
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
                    this.SimulationEffect.Velocity = emitter.Velocity.ReadTarget;
                    //this.SimulationEffect.Acceleration = emitter.Acceleration.ReadTarget;
                    this.SimulationEffect.Position = emitter.Position.ReadTarget;

                    this.SimulationEffect.LengthScale = emitter.LengthScale;
                    this.SimulationEffect.FieldSpeed = emitter.FieldSpeed;
                    this.SimulationEffect.NoiseStrength = emitter.NoiseStrength;
                    this.SimulationEffect.ProgressionRate = emitter.ProgressionRate;
                    this.SimulationEffect.FieldMainDirection = emitter.FieldMainDirection;
                    this.SimulationEffect.SpherePosition = emitter.SpherePosition;
                    this.SimulationEffect.SphereRadius = emitter.SphereRadius;
                    this.SimulationEffect.EmitterSize = emitter.Size;
                    this.SimulationEffect.MaxLifeTime = emitter.MaxLifeTime;

                    this.SimulationEffect.Elapsed = this.FrameService.Elapsed;
                    this.SimulationEffect.Time = this.FrameService.Time;

                    //// Render accelerations
                    //this.SimulationEffect.ApplyAcceleration();
                    //this.Device.SetRenderTarget(emitter.Acceleration.WriteTarget);
                    //this.PostProcessTriangle.Render(this.Device);

                    // Render velocities
                    this.SimulationEffect.ApplyVelocity();
                    this.Device.SetRenderTarget(emitter.Velocity.WriteTarget);
                    this.PostProcessTriangle.Render(this.Device);

                    // Update positions
                    this.SimulationEffect.ApplyPosition();
                    this.Device.SetRenderTarget(emitter.Position.WriteTarget);
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
            this.Effect.Data = emitter.Position.ReadTarget;

            this.Effect.Apply();
        }
    }
}
