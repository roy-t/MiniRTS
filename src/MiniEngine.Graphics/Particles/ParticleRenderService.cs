using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.Physics;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.Graphics.Particles
{
    [Service]
    public sealed class ParticleRenderService : IRenderService
    {
        private readonly IComponentContainer<ParticleEmitterComponent> Particles;
        private readonly IComponentContainer<TransformComponent> Transforms;

        private readonly ParticleEffect GBufferEffect;
        private readonly ParticleShadowMapEffect ShadowMapEffect;

        private readonly GraphicsDevice Device;
        private readonly Point Point;

        public ParticleRenderService(IComponentContainer<ParticleEmitterComponent> particles, IComponentContainer<TransformComponent> transforms, ParticleEffect gBufferEffect, ParticleShadowMapEffect shadowMapEffect, GraphicsDevice device, Point point)
        {
            this.Particles = particles;
            this.Transforms = transforms;
            this.GBufferEffect = gBufferEffect;
            this.ShadowMapEffect = shadowMapEffect;
            this.Device = device;
            this.Point = point;
        }

        public void DrawToShadowMap(Matrix viewProjection, Entity entity)
        {
            var emitter = this.Particles.Get(entity);
            var transform = this.Transforms.Get(entity);

            this.ApplyShadowMapEffect(viewProjection, transform.Transform, emitter);
            this.Point.RenderInstanced(this.Device, emitter.Instances, emitter.Count);
        }

        public void DrawToGBuffer(PerspectiveCamera camera, Entity entity)
        {
            var emitter = this.Particles.Get(entity);
            var transform = this.Transforms.Get(entity);

            this.ApplyGBufferEffect(camera, transform.Transform, emitter);
            this.Point.RenderInstanced(this.Device, emitter.Instances, emitter.Count);
        }

        private void ApplyGBufferEffect(PerspectiveCamera camera, Transform transform, ParticleEmitterComponent emitter)
        {
            this.GBufferEffect.View = camera.View;
            this.GBufferEffect.WorldViewProjection = transform.Matrix * camera.ViewProjection;
            this.GBufferEffect.Metalicness = emitter.Metalicness;
            this.GBufferEffect.Roughness = emitter.Roughness;
            this.GBufferEffect.Position = emitter.Position.ReadTarget;
            this.GBufferEffect.Velocity = emitter.Velocity.ReadTarget;
            this.GBufferEffect.SlowColor = emitter.SlowColor.ToVector3();
            this.GBufferEffect.FastColor = emitter.FastColor.ToVector3();
            this.GBufferEffect.ColorVelocityModifier = emitter.ColorVelocityModifier;

            this.GBufferEffect.Apply();
        }

        private void ApplyShadowMapEffect(Matrix viewProjection, Transform transform, ParticleEmitterComponent emitter)
        {
            this.ShadowMapEffect.WorldViewProjection = transform.Matrix * viewProjection;
            this.ShadowMapEffect.Data = emitter.Position.ReadTarget;

            this.ShadowMapEffect.Apply();
        }
    }
}
