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

        private Particle[] instanceData;
        private VertexBuffer instanceBuffer;

        public ParticleBuffer(GraphicsDevice device)
        {
            this.Device = device;

            this.instanceData = new Particle[InitialBufferSize];

            this.VertexDeclaration = Particle.Declaration;
            this.instanceBuffer = new VertexBuffer(this.Device, this.VertexDeclaration, this.instanceData.Length, BufferUsage.WriteOnly);
        }

        public int Count { get; private set; }

        public ref Particle this[int index] => ref this.instanceData[index];

        public Span<Particle> Create(int count)
        {
            this.EnsureSpace(this.Count + count);

            var span = new Span<Particle>(this.instanceData, this.Count, count);
            this.Count += count;

            return span;
        }

        public void RemoveAt(int index)
        {
            this.instanceData[index] = this.instanceData[this.Count - 1];
            this.Count--;
        }

        public VertexBuffer Commit()
        {
            if (this.Count > 0)
            {
                this.instanceBuffer.SetData(0, this.instanceData, 0, this.Count, this.VertexDeclaration.VertexStride);
            }

            return this.instanceBuffer;
        }

        private void EnsureSpace(int space)
        {
            if (space < this.instanceData.Length)
            {
                return;
            }

            var size = (int)Math.Ceiling(Math.Max(space + MinimumGrowth, space * GrowthFactor));

            var instanceBuffer = new Particle[size];
            this.instanceData.CopyTo(instanceBuffer, 0);
            this.instanceData = instanceBuffer;


            this.instanceBuffer.Dispose();
            this.instanceBuffer = new VertexBuffer(this.Device, this.VertexDeclaration, size, BufferUsage.WriteOnly);
        }

        public void Dispose() => this.instanceBuffer.Dispose();
    }
}
