using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Particles.Systems
{
    public sealed class AdditiveParticleSystem : IUpdatableSystem
    {
        private readonly GraphicsDevice Device;
        private readonly AdditiveParticlesEffect AdditiveParticlesEffect;

        private readonly IComponentContainer<AdditiveEmitter> Emitters;
        private readonly IComponentContainer<Pose> Poses;
        private readonly List<ParticlePose> Particles;

        private readonly UnitQuad Quad;

        public AdditiveParticleSystem(
            GraphicsDevice device,
            EffectFactory effectFactory,
            IComponentContainer<AdditiveEmitter> emitters,
            IComponentContainer<Pose> poses)
        {
            this.Device = device;

            this.AdditiveParticlesEffect = effectFactory.Construct<AdditiveParticlesEffect>();
            this.Emitters = emitters;
            this.Poses = poses;

            this.Particles = new List<ParticlePose>();
            this.Quad = new UnitQuad(device);
        }

        public void RenderAdditiveParticles(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Particles.Clear();

            ParticleHelper.GatherParticles(this.Emitters, camera, this.Particles);

            this.Device.AdditiveParticleState();

            for (var i = 0; i < this.Particles.Count; i++)
            {
                var particle = this.Particles[i];

                this.AdditiveParticlesEffect.World = particle.Pose;
                this.AdditiveParticlesEffect.View = camera.View;
                this.AdditiveParticlesEffect.Projection = camera.Projection;
                this.AdditiveParticlesEffect.DiffuseMap = particle.Texture;
                this.AdditiveParticlesEffect.Tint = particle.Tint.ToVector4();
                this.AdditiveParticlesEffect.DepthMap = gBuffer.DepthTarget;
                this.AdditiveParticlesEffect.InverseViewProjection = camera.InverseViewProjection;

                this.AdditiveParticlesEffect.Apply();

                this.Quad.SetTextureCoordinates(particle.MinUv, particle.MaxUv);
                this.Quad.Render();
            }
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            for (var i = 0; i < this.Emitters.Count; i++)
            {
                var emitter = this.Emitters[i];
                var pose = this.Poses.Get(emitter.Entity);
                emitter.Update(elapsed, pose);
            }
        }
    }
}
