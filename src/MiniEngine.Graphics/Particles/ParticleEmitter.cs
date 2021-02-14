using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEmitter : IDisposable
    {
        public ParticleEmitter(ParticleBuffer particles, Texture2D texture, IParticleSpawnFunction spawnFunction, IParticleUpdateFunction updateFunction)
        {
            this.Particles = particles;
            this.Texture = texture;
            this.SpawnFunction = spawnFunction;
            this.UpdateFunction = updateFunction;
        }

        public ParticleBuffer Particles { get; }

        public Texture2D Texture { get; set; }

        public IParticleSpawnFunction SpawnFunction { get; set; }

        public IParticleUpdateFunction UpdateFunction { get; set; }

        public void Update(float elapsed, Matrix transform, ICamera camera)
        {
            this.RemoveOldParticles(elapsed);
            this.SpawnNewParticles(elapsed, transform);
            this.UpdateParticles(elapsed, camera);
        }

        public void RemoveOldParticles(float elapsed)
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

        public void SpawnNewParticles(float elapsed, Matrix transform)
            => this.Particles.Add(this.SpawnFunction.Spawn(elapsed, transform));

        public void UpdateParticles(float elapsed, ICamera camera)
        {
            for (var i = this.Particles.Count - 1; i >= 0; i--)
            {
                this.UpdateFunction.Update(elapsed, ref this.Particles[i], camera);
            }
        }

        public void Dispose() => this.Particles.Dispose();
    }
}
