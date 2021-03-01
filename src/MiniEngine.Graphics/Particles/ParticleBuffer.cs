using System;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleBuffer : IDisposable
    {
        private const int InitialBufferSize = 20;
        private const int MinimumGrowth = 10;
        private const float GrowthFactor = 1.2f;

        private readonly GraphicsDevice Device;
        private readonly VertexDeclaration VertexDeclaration;

        private Particle[] particles;
        private ParticleInstancingVertex[] instanceData;
        private VertexBuffer instanceBuffer;

        public ParticleBuffer(GraphicsDevice device)
        {
            this.Device = device;

            this.particles = new Particle[InitialBufferSize];
            this.instanceData = new ParticleInstancingVertex[this.particles.Length];

            this.VertexDeclaration = ParticleInstancingVertex.Declaration;
            this.instanceBuffer = new VertexBuffer(this.Device, this.VertexDeclaration, this.particles.Length, BufferUsage.WriteOnly);
        }

        public int Count { get; private set; }

        public ref Particle this[int index] => ref this.particles[index];

        public Span<Particle> Create(int count)
        {
            this.EnsureSpace(this.Count + count);

            var span = new Span<Particle>(this.particles, this.Count, count);
            this.Count += count;

            return span;
        }

        public void RemoveAt(int index)
        {
            this.particles[index] = this.particles[this.Count - 1];
            this.Count--;
        }

        public VertexBuffer Commit()
        {
            if (this.Count > 0)
            {
                for (var i = 0; i < this.Count; i++)
                {
                    this.instanceData[i].Position = this.particles[i].Position;
                    this.instanceData[i].Color = this.particles[i].Color.ToVector3();
                    this.instanceData[i].Scale = this.particles[i].Scale;
                    this.instanceData[i].Metalicness = this.particles[i].Metalicness;
                    this.instanceData[i].Roughness = this.particles[i].Roughness;
                }

                this.instanceBuffer.SetData(this.instanceData, 0, this.Count);
            }

            return this.instanceBuffer;
        }

        private void EnsureSpace(int space)
        {
            if (space < this.particles.Length)
            {
                return;
            }

            var size = (int)Math.Ceiling(Math.Max(space + MinimumGrowth, space * GrowthFactor));

            var particleBuffer = new Particle[size];
            this.particles.CopyTo(particleBuffer, 0);
            this.particles = particleBuffer;

            this.instanceData = new ParticleInstancingVertex[size];

            this.instanceBuffer.Dispose();
            this.instanceBuffer = new VertexBuffer(this.Device, this.VertexDeclaration, size, BufferUsage.WriteOnly);
        }

        public void Dispose() => this.instanceBuffer.Dispose();
    }
}
