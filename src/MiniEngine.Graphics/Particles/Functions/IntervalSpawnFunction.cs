using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class IntervalSpawnFunction : IParticleSpawnFunction
    {
        private float counter;

        public IntervalSpawnFunction()
        {
            this.Tint = Color.White;
            this.SpawnInterval = 1.0f;
        }

        public Color Tint { get; set; }

        public float SpawnInterval { get; set; }

        public void Spawn(float elapsed, Matrix transform, ParticleBuffer buffer)
        {
            this.counter += elapsed;
            if (this.counter >= this.SpawnInterval)
            {
                var created = buffer.Create(1);
                created[0].Position = transform.Translation;
                created[0].Color = this.Tint;
                created[0].Energy = 1.0f;

                this.counter -= this.SpawnInterval;
            }
        }
    }
}
