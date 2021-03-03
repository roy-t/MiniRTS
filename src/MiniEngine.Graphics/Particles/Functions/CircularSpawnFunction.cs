using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class CircularSpawnFunction : IParticleSpawnFunction
    {
        private static Random Random = new Random();

        private float counter;
        private float angle;

        public CircularSpawnFunction()
        {
            this.Radius = 1.0f;
            this.ParticlesPerWave = 3.0f;
            this.SpawnInterval = 0.01f;

            this.Scale = 1.0f;
            this.Tint = Color.White;
            this.Metalicness = 0.5f;
            this.Roughness = 0.5f;
        }

        public float Radius { get; set; }

        public float ParticlesPerWave { get; set; }

        public float SpawnInterval { get; set; }

        // TODO: common base class?
        public float Scale { get; set; }

        public Color Tint { get; set; }

        public float Metalicness { get; set; }

        public float Roughness { get; set; }

        public void Spawn(float elapsed, Matrix transform, ParticleBuffer buffer)
        {
            this.counter += elapsed;
            if (this.counter >= this.SpawnInterval)
            {
                var created = buffer.Create((int)((this.counter * this.ParticlesPerWave) / this.SpawnInterval));

                for (var i = 0; i < created.Length; i++)
                {
                    this.angle += (float)(Random.NextDouble()) * MathHelper.TwoPi;
                    var rotation = Matrix.CreateFromAxisAngle(transform.Forward, this.angle);
                    var position = Vector3.TransformNormal(transform.Left, rotation) * this.Radius;

                    created[i].Position = transform.Translation + position;
                    created[i].Scale = this.Scale;
                    created[i].Color = this.Tint;
                    created[i].Metalicness = this.Metalicness;
                    created[i].Roughness = this.Roughness;
                    created[i].Energy = 1.0f;
                }

                this.counter -= this.SpawnInterval;
            }
        }
    }
}
