using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class InstantSpawnFunction : IParticleSpawnFunction
    {
        public InstantSpawnFunction(int count = 375_000)
        {
            this.Count = count;
            this.Enabled = true;
        }

        public int Count { get; }

        public bool Enabled { get; set; }

        public Particle[] Spawn(float elapsed, Matrix transform)
        {
            if (this.Enabled)
            {
                this.Enabled = false;

                return Enumerable.Repeat(NewParticle(transform), this.Count).ToArray();
            }
            else
            {
                return Array.Empty<Particle>();
            }
        }

        private static Particle NewParticle(Matrix transform)
        {
            return new Particle()
            {
                Position = transform.Translation,
                Scale = 1.0f,
                Color = Color.White,
                Energy = 100.0f
            };
        }
    }
}
