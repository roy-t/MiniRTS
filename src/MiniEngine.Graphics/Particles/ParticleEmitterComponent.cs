using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEmitterComponent : AComponent, IDisposable
    {
        private const int InitialBufferSize = 20;
        private const int MinimumGrowth = 10;
        private const float GrowthFactor = 1.2f;

        private readonly GraphicsDevice Device;
        private Particle[] particles;
        private InstancingVertex[] instanceData;

        public ParticleEmitterComponent(Entity entity, IParticleSpawnFunction spawnFunction, IParticleUpdateFunction updateFunction, GraphicsDevice device, Texture2D texture)
            : base(entity)
        {
            this.particles = new Particle[InitialBufferSize];
            this.instanceData = new InstancingVertex[this.particles.Length];
            this.InstanceBuffer = new VertexBuffer(device, InstancingVertex.Declaration, this.particles.Length, BufferUsage.None);
            this.SpawnFunction = spawnFunction;
            this.UpdateFunction = updateFunction;
            this.Device = device;
            this.Texture = texture;
        }

        public int Count { get; private set; }

        public Texture2D Texture { get; set; }

        public IParticleSpawnFunction SpawnFunction { get; set; }

        public IParticleUpdateFunction UpdateFunction { get; set; }

        public VertexBuffer InstanceBuffer { get; private set; }

        public void Update(float elapsed, Matrix transform)
        {
            this.RemoveOldParticles(elapsed);
            this.SpawnNewParticles(elapsed, transform);
            this.UpdateParticles(elapsed);
        }

        private void RemoveOldParticles(float elapsed)
        {
            for (var i = this.Count - 1; i >= 0; i--)
            {
                this.particles[i].TimeToLive -= elapsed;
                if (this.particles[i].TimeToLive <= 0)
                {
                    this.particles[i] = this.particles[this.Count - 1];
                    this.Count--;
                }
            }
        }

        private void SpawnNewParticles(float elapsed, Matrix transform)
        {
            var newParticles = this.SpawnFunction.Spawn(elapsed, transform);
            this.EnsureSpace(this.Count + newParticles.Length);

            newParticles.CopyTo(this.particles, this.Count);

            this.Count += newParticles.Length;
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

            this.InstanceBuffer?.Dispose();
            this.InstanceBuffer = new VertexBuffer(this.Device, InstancingVertex.Declaration, size, BufferUsage.None);
        }

        private void UpdateParticles(float elapsed)
        {
            for (var i = this.Count - 1; i >= 0; i--)
            {
                this.UpdateFunction.Update(elapsed, ref this.particles[i]);
                this.instanceData[i] = new InstancingVertex(this.particles[i].Transform);
            }

            if (this.Count > 0)
            {
                this.InstanceBuffer.SetData(this.instanceData, 0, this.Count);
            }
        }

        public void Dispose() => this.InstanceBuffer.Dispose();
    }
}