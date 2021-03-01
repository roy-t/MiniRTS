using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEmitter : IDisposable
    {
        public ParticleEmitter(ParticleBuffer particles, IParticleSpawnFunction spawnFunction, IParticleUpdateFunction updateFunction)
        {
            this.Particles = particles;
            this.SpawnFunction = spawnFunction;
            this.UpdateFunction = updateFunction;

            this.TimeToLive = 10.0f;
        }

        public ParticleBuffer Particles { get; }

        public IParticleSpawnFunction SpawnFunction { get; set; }

        public IParticleUpdateFunction UpdateFunction { get; set; }

        public float TimeToLive { get; set; }

        public void Update(float elapsed, Matrix transform)
        {
            this.RemoveOldParticles(elapsed);
            this.SpawnNewParticles(elapsed, transform);
            this.UpdateParticles(elapsed, transform);
        }

        public void RemoveOldParticles(float elapsed)
        {
            for (var i = this.Particles.Count - 1; i >= 0; i--)
            {
                ref var particle = ref this.Particles[i];
                particle.Energy -= elapsed / this.TimeToLive;
                if (particle.Energy <= 0.0f)
                {
                    this.Particles.RemoveAt(i);
                }
            }
        }

        public void SpawnNewParticles(float elapsed, Matrix transform)
            => this.SpawnFunction.Spawn(elapsed, transform, this.Particles);

        public void UpdateParticles(float elapsed, Matrix transform)
        {
            for (var i = this.Particles.Count - 1; i >= 0; i--)
            {
                this.UpdateFunction.Update(elapsed, transform, ref this.Particles[i]);
            }
        }

        public void Dispose() => this.Particles.Dispose();
    }
}
