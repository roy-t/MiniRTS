using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class IntervalSpawnFunction : IParticleSpawnFunction
    {
        private float counter;

        public IntervalSpawnFunction()
        {
            // TODO: remove unused
            this.Velocity = 1.0f;
            this.Angle = 0.0f;
            this.Amplitude = 0.0f;
            this.Scale = 1.0f;
            this.Tint = Color.White;
            this.MaxAge = 10.0f;
            this.SpawnInterval = 1.0f;
        }

        public float Velocity { get; set; }

        public float Angle { get; set; }

        public float Amplitude { get; set; }

        public float Scale { get; set; }

        public Color Tint { get; set; }

        public float MaxAge { get; set; }

        public float SpawnInterval { get; set; }

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
            return new Particle()
            {
                Position = transform.Translation,
                Scale = this.Scale,
                Color = this.Tint,
                Energy = this.MaxAge,
            };
        }
    }
}
