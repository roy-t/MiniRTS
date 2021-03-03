using System;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class RandomizedDespawnFunction : IParticleDespawnFunction
    {

        public RandomizedDespawnFunction()
        {
            this.Variance = 0.1f;
            this.AverageLifetime = 10.0f;
        }

        public float Variance { get; set; }

        public float AverageLifetime { get; set; }

        public void Update(int id, float elapsed, ref Particle particle)
        {
            var random = new Random(id);
            var r = ((float)random.NextDouble() - 0.5f) * 2.0f;
            r = 1.0f + (r * this.Variance);
            r /= this.AverageLifetime;
            particle.Energy -= elapsed * r;
        }
    }
}
