﻿using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Geometry;

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
        private InstancingVertex[] instanceData;
        private VertexBuffer instanceBuffer;

        public ParticleBuffer(GraphicsDevice device)
        {
            this.Device = device;

            this.particles = new Particle[InitialBufferSize];
            this.instanceData = new InstancingVertex[this.particles.Length];

            this.VertexDeclaration = InstancingVertex.Declaration;
            this.instanceBuffer = new VertexBuffer(this.Device, this.VertexDeclaration, this.particles.Length, BufferUsage.WriteOnly);
        }

        public int Count { get; private set; }

        public ref Particle this[int index] => ref this.particles[index];

        public void Add(Particle[] particles)
        {
            this.EnsureSpace(this.Count + particles.Length);

            particles.CopyTo(this.particles, this.Count);
            this.Count += particles.Length;
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
                    this.instanceData[i].Transform = this.particles[i].Transform;
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

            var size = Math.Max(this.particles.Length + MinimumGrowth, (int)(this.particles.Length * GrowthFactor));

            var particleBuffer = new Particle[size];
            this.particles.CopyTo(particleBuffer, 0);
            this.particles = particleBuffer;

            this.instanceData = new InstancingVertex[size];

            this.instanceBuffer.Dispose();
            this.instanceBuffer = new VertexBuffer(this.Device, this.VertexDeclaration, size, BufferUsage.WriteOnly);
        }

        public void Dispose() => this.instanceBuffer.Dispose();
    }
}
