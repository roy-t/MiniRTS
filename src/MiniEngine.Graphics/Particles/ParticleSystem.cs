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
        }

        [ProcessAll]
        public void SimulateParticles(ParticleEmitterComponent component)
        {
            if (component.IsEnabled)
            {
                this.SimulationEffect.Velocity = component.Velocity.ReadTarget;
                this.SimulationEffect.Position = component.Position.ReadTarget;

                this.SimulationEffect.LengthScale = component.LengthScale;
                this.SimulationEffect.FieldSpeed = component.FieldSpeed;
                this.SimulationEffect.NoiseStrength = component.NoiseStrength;
                this.SimulationEffect.ProgressionRate = component.ProgressionRate;
                this.SimulationEffect.SpherePosition = component.SpherePosition;
                this.SimulationEffect.SphereRadius = component.SphereRadius;
                this.SimulationEffect.EmitterSize = component.Size;
                this.SimulationEffect.MaxLifeTime = component.MaxLifeTime;

                this.SimulationEffect.Elapsed = this.FrameService.Elapsed;
                this.SimulationEffect.Time = this.FrameService.Time;

                this.SimulationEffect.ApplyVelocity();
                this.Device.SetRenderTarget(component.Velocity.WriteTarget);
                this.PostProcessTriangle.Render(this.Device);

                this.SimulationEffect.ApplyPosition();
                this.Device.SetRenderTarget(component.Position.WriteTarget);
                this.PostProcessTriangle.Render(this.Device);

                component.Swap();
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

        public void ApplyEffect(Matrix worldViewProjection, ParticleEmitterComponent emitter)
        {
            this.Effect.WorldViewProjection = worldViewProjection;
            this.Effect.Metalicness = emitter.Metalicness;
            this.Effect.Roughness = emitter.Roughness;
            this.Effect.Position = emitter.Position.ReadTarget;
            this.Effect.Velocity = emitter.Velocity.ReadTarget;
            this.Effect.SlowColor = emitter.SlowColor;
            this.Effect.FastColor = emitter.FastColor;
            this.Effect.ColorVelocityModifier = emitter.ColorVelocityModifier;

            this.Effect.Apply();
        }
    }
}
