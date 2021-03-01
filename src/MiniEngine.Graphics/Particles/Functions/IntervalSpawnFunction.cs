using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class IntervalSpawnFunction : IParticleSpawnFunction
    {
        private float counter;

        public IntervalSpawnFunction()
        {
            this.Scale = 1.0f;
            this.Tint = Color.White;
            this.SpawnInterval = 1.0f;
            this.Metalicness = 0.5f;
            this.Roughness = 0.5f;
        }

        public float Scale { get; set; }

        public Color Tint { get; set; }

        public float Metalicness { get; set; }

        public float Roughness { get; set; }

        public float SpawnInterval { get; set; }

        public void Spawn(float elapsed, Matrix transform, ParticleBuffer buffer)
        {
            this.counter += elapsed;
            if (this.counter >= this.SpawnInterval)
            {
                var created = buffer.Create(1);
                created[0].Position = transform.Translation;
                created[0].Scale = this.Scale;
                created[0].Color = this.Tint;
                created[0].Metalicness = this.Metalicness;
                created[0].Roughness = this.Roughness;
                created[0].Energy = 1.0f;

                this.counter -= this.SpawnInterval;
            }
        }
    }
}
