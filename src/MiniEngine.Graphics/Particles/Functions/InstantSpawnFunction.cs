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

        public void Spawn(float elapsed, Matrix transform, ParticleBuffer buffer)
        {
            if (this.Enabled)
            {
                this.Enabled = false;

                var created = buffer.Create(this.Count);
                for (var i = 0; i < created.Length; i++)
                {
                    created[i].Position = transform.Translation;
                    created[i].Color = Color.White;
                    created[i].Energy = 1.0f;
                }
            }
        }
    }
}
