using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Particles.Systems
{
    public sealed class AdditiveParticleSystem : IUpdatableSystem
    {
        private readonly GraphicsDevice Device;
        private readonly AdditiveParticlesEffect AdditiveParticlesEffect;

        private readonly EntityLinker Linker;
        private readonly List<AdditiveEmitter> Emitters;
        private readonly List<ParticlePose> Particles;

        private readonly FullScreenQuad Quad;

        public AdditiveParticleSystem(
            GraphicsDevice device,
            AdditiveParticlesEffect additiveParticlesEffect,
            EntityLinker linker)
        {
            this.Device = device;

            this.AdditiveParticlesEffect = additiveParticlesEffect;

            this.Linker = linker;
            this.Emitters = new List<AdditiveEmitter>();

            this.Particles = new List<ParticlePose>();

            this.Quad = new FullScreenQuad(device);
        }

        public void RenderAdditiveParticles(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Particles.Clear();
            this.Emitters.Clear();
            this.Linker.GetComponents(this.Emitters);

            ParticleHelper.GatherParticles(this.Emitters, camera, this.Particles);

            using (this.Device.AdditiveParticleState())
            {
                foreach (var particle in this.Particles)
                {
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
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Emitters.Clear();
            this.Linker.GetComponents(this.Emitters);

            foreach (var emitter in this.Emitters)
            {
                emitter.Update(elapsed);
            }
        }
    }
}
