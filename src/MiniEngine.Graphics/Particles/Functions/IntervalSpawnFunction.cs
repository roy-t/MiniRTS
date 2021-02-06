using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class IntervalSpawnFunction : IParticleSpawnFunction
    {
        private float counter;

        public IntervalSpawnFunction()
        {
            this.SpawnInterval = 1.0f;
            this.Velocity = 1.0f;
        }

        public float SpawnInterval { get; set; }

        public float Velocity { get; set; }

        public Particle[] Spawn(float elapsed, Matrix transform)
        {
            this.counter += elapsed;
            if (this.counter < this.SpawnInterval)
            {
                return Array.Empty<Particle>();
            }

            this.counter -= this.SpawnInterval;

            return new Particle[] { this.NewParticle(transform) };
        }

        private Particle NewParticle(Matrix transform)
        {
            transform.Decompose(out var scale, out _, out _);
            return new Particle()
            {
                StartPosition = transform.Translation,
                Velocity = transform.Forward * this.Velocity,
                MaxAge = 10.0f,
                Scale = scale
            };
        }
    }
}
