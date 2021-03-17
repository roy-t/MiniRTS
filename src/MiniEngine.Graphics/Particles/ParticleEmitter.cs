using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEmitter : IDisposable
    {
        private static readonly Random R = new();

        public ParticleEmitter(GraphicsDevice device, int count)
        {
            this.Metalicness = 0.0f;
            this.Roughness = 1.0f;

            var dimensions = (int)Math.Ceiling(Math.Sqrt(count));
            this.Count = dimensions * dimensions;

            this.Velocity = new DoubleBufferedRenderTarget(device, dimensions, SurfaceFormat.HalfVector4);
            this.Position = new DoubleBufferedRenderTarget(device, dimensions, SurfaceFormat.Vector4);

            var instances = new Particle[this.Count];
            var i = 0;
            for (var x = 0; x < dimensions; x++)
            {
                for (var y = 0; y < dimensions; y++)
                {
                    var u = x / (float)dimensions;
                    var v = y / (float)dimensions;

                    instances[i++] = new Particle(new Vector2(u, v));
                }
            }
            this.Instances = new VertexBuffer(device, PointVertex.Declaration, count, BufferUsage.WriteOnly);
            this.Instances.SetData(instances);

            this.SeedData();
        }

        public bool Reset
        {
            get { return false; }
            set { if (value) { this.SeedData(); } }
        }

        public float Metalicness { get; set; }
        public float Roughness { get; set; }

        public float Size { get; set; } = 1.0f;
        public float MaxLifeTime { get; set; } = 3.0f;

        public float LengthScale { get; set; } = 0.5f;
        public float FieldSpeed { get; set; } = 0.5f;
        public float NoiseStrength { get; set; } = 0.3f;
        public float ProgressionRate { get; set; } = 1.0f;
        public Vector3 FieldMainDirection { get; set; } = Vector3.Forward;

        public Vector3 SpherePosition { get; set; } = Vector3.Forward;
        public float SphereRadius { get; set; } = 0.5f;

        public int Count { get; }

        public VertexBuffer Instances { get; }

        public DoubleBufferedRenderTarget Velocity { get; }
        public DoubleBufferedRenderTarget Position { get; }

        public void Swap()
        {
            this.Velocity.Swap();
            this.Position.Swap();
        }

        private void SeedData()
        {
            var data = new Vector4[this.Count];

            for (var i = 0; i < this.Count; i++)
            {
                var a = R.NextDouble() * MathHelper.TwoPi;
                var r = Math.Sqrt(R.NextDouble()) * this.Size;
                var x = r * Math.Cos(a);
                var y = r * Math.Sin(a);

                var age = (R.NextDouble() * this.MaxLifeTime) - this.MaxLifeTime;

                data[i] = new Vector4((float)x, (float)y, 0.0f, (float)age);

            }

            this.Position.WriteTarget.SetData(data);
            this.Swap();
        }

        public void Dispose()
        {
            this.Velocity.Dispose();
            this.Position.Dispose();
        }
    }
}
