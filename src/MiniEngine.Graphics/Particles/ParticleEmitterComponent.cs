using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEmitterComponent : AComponent, IDisposable
    {
        public ParticleEmitterComponent(Entity entity, IParticleSpawnFunction spawnFunction, IParticleUpdateFunction updateFunction, GraphicsDevice device, Texture2D texture)
            : base(entity)
        {
            this.SpawnFunction = spawnFunction;
            this.UpdateFunction = updateFunction;
            this.Texture = texture;

            this.Particles = new ParticleBuffer(device);
        }

        public ParticleBuffer Particles { get; }

        public Texture2D Texture { get; set; }

        public IParticleSpawnFunction SpawnFunction { get; set; }

        public IParticleUpdateFunction UpdateFunction { get; set; }

        public void Update(float elapsed, Matrix transform, ICamera camera)
        {
            this.RemoveOldParticles(elapsed);
            this.SpawnNewParticles(elapsed, transform);
            this.UpdateParticles(camera);
        }

        private void RemoveOldParticles(float elapsed)
        {
            for (var i = this.Particles.Count - 1; i >= 0; i--)
            {
                ref var particle = ref this.Particles[i];
                particle.Age += elapsed;
                if (particle.Age >= particle.MaxAge)
                {
                    this.Particles.RemoveAt(i);
                }
            }
        }

        private void SpawnNewParticles(float elapsed, Matrix transform)
            => this.Particles.Add(this.SpawnFunction.Spawn(elapsed, transform));

        private void UpdateParticles(ICamera camera)
        {
            for (var i = this.Particles.Count - 1; i >= 0; i--)
            {
                this.UpdateFunction.Update(ref this.Particles[i], camera);
            }
        }

        public void Dispose() => this.Particles.Dispose();
    }
}