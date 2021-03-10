using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEmitter : IDisposable
    {
        private bool swapped;
        private readonly RenderTarget2D DataA;
        private readonly RenderTarget2D DataB;

        public ParticleEmitter(GraphicsDevice device, int count)
        {
            this.Metalicness = 0.0f;
            this.Roughness = 1.0f;

            var dimensions = (int)Math.Ceiling(Math.Sqrt(count));
            this.Count = dimensions * dimensions;
            this.DataA = new RenderTarget2D(device, dimensions, dimensions, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this.DataB = new RenderTarget2D(device, dimensions, dimensions, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

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

            this.GenerateSpawnPositions();
        }

        public float Metalicness { get; set; }
        public float Roughness { get; set; }

        public int Count { get; }

        public VertexBuffer Instances { get; }

        public RenderTarget2D FrontBuffer
        {
            get
            {
                if (this.swapped)
                {
                    return this.DataB;
                }

                return this.DataA;
            }
        }

        public RenderTarget2D BackBuffer
        {
            get
            {
                if (this.swapped)
                {
                    return this.DataA;
                }

                return this.DataB;
            }
        }

        public void Swap()
            => this.swapped = !this.swapped;


        private void GenerateSpawnPositions()
        {
            var random = new Random();
            var data = new Vector4[this.Count];

            for (var i = 0; i < this.Count; i++)
            {
                var x = (float)((random.NextDouble() * 2) - 1);
                var y = (float)((random.NextDouble() * 2) - 1);
                var z = (float)((random.NextDouble() * 2) - 1);

                data[i] = new Vector4(x, y, z, 1.0f);
            }

            this.DataA.SetData(data);
            this.DataB.SetData(data);
        }

        public void Dispose()
        {
            this.DataA.Dispose();
            this.DataB.Dispose();
        }
    }
}
