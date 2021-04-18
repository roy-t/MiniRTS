using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.Graphics.Particles
{
    [Service]
    public sealed class ParticleRenderService : IRenderService
    {
        private readonly IComponentContainer<ParticleEmitterComponent> Particles;

        private readonly ParticleEffect GBufferEffect;
        private readonly ParticleShadowMapEffect ShadowMapEffect;

        private readonly GraphicsDevice Device;
        private readonly Point Point;

        public ParticleRenderService(IComponentContainer<ParticleEmitterComponent> particles, ParticleEffect gBufferEffect, ParticleShadowMapEffect shadowMapEffect, GraphicsDevice device, Point point)
        {
            this.Particles = particles;
            this.GBufferEffect = gBufferEffect;
            this.ShadowMapEffect = shadowMapEffect;
            this.Device = device;
            this.Point = point;
        }

        public void DrawToShadowMap(Matrix viewProjection, Entity entity)
        {
            var emitter = this.Particles.Get(entity);

            this.ApplyShadowMapEffect(viewProjection, emitter);
            this.Point.RenderInstanced(this.Device, emitter.Instances, emitter.Count);
        }

        public void DrawToGBuffer(PerspectiveCamera camera, Entity entity)
        {
            var emitter = this.Particles.Get(entity);

            this.ApplyGBufferEffect(camera, emitter);
            this.Point.RenderInstanced(this.Device, emitter.Instances, emitter.Count);
        }

        private void ApplyGBufferEffect(PerspectiveCamera camera, ParticleEmitterComponent emitter)
        {
            this.GBufferEffect.View = camera.View;
            this.GBufferEffect.WorldViewProjection = camera.ViewProjection;
            this.GBufferEffect.Metalicness = emitter.Metalicness;
            this.GBufferEffect.Roughness = emitter.Roughness;
            this.GBufferEffect.Position = emitter.Position.ReadTarget;
            this.GBufferEffect.Velocity = emitter.Velocity.ReadTarget;
            this.GBufferEffect.SlowColor = emitter.SlowColor.ToVector3();
            this.GBufferEffect.FastColor = emitter.FastColor.ToVector3();
            this.GBufferEffect.ColorVelocityModifier = emitter.ColorVelocityModifier;

            this.GBufferEffect.Apply();
        }

        private void ApplyShadowMapEffect(Matrix viewProjection, ParticleEmitterComponent emitter)
        {
            this.ShadowMapEffect.WorldViewProjection = viewProjection;
            this.ShadowMapEffect.Data = emitter.Position.ReadTarget;

            this.ShadowMapEffect.Apply();
        }
    }
}
