using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.Physics;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Particles
{
    [System]
    public partial class ParticleSimulationSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly ParticleSimulationEffect SimulationEffect;

        public ParticleSimulationSystem(GraphicsDevice device, FrameService frameService, PostProcessTriangle postProcessTriangle, ParticleSimulationEffect simulationEffect)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.PostProcessTriangle = postProcessTriangle;
            this.SimulationEffect = simulationEffect;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        [ProcessNew]
        public void SetBounds(ParticleEmitterComponent component, BoundingSphereComponent bounds)
        {
            bounds.Radius = ComputeBounds(component);
            bounds.ChangeState.Change();
        }

        [ProcessChanged]
        public void UpdateBounds(ParticleEmitterComponent component, BoundingSphereComponent bounds)
        {
            bounds.Radius = ComputeBounds(component);
            bounds.ChangeState.Change();
        }

        [ProcessAll]
        public void SimulateParticles(ParticleEmitterComponent component, ForcesComponent forces, TransformComponent transform)
        {
            if (component.IsEnabled)
            {
                this.SimulationEffect.Velocity = component.Velocity.ReadTarget;
                this.SimulationEffect.Position = component.Position.ReadTarget;
                this.SimulationEffect.InitialVelocity = component.InitialVelocity.ReadTarget;

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

                this.SimulationEffect.ParentVelocity = forces.Velocity;
                this.SimulationEffect.ObjectToWorld = transform.Matrix;
                this.SimulationEffect.WorldToObject = Matrix.Invert(transform.Matrix);

                this.SimulationEffect.ApplyParticleVelocitySimulationTechnique();
                this.Device.SetRenderTarget(component.Velocity.WriteTarget);
                this.PostProcessTriangle.Render(this.Device);

                this.SimulationEffect.ApplyParticlePositionSimulationTechnique();
                this.Device.SetRenderTargets(component.Position.WriteTarget, component.InitialVelocity.WriteTarget);
                this.PostProcessTriangle.Render(this.Device);

                component.Swap();
            }
        }

        private static float ComputeBounds(ParticleEmitterComponent component) => Math.Max(component.Size, component.FieldSpeed * 2 * component.MaxLifeTime);
    }
}
