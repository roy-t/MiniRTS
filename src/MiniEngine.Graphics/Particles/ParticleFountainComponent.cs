using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleFountainComponent : AComponent, IDisposable
    {
        private readonly GraphicsDevice Device;
        private readonly List<ParticleEmitter> EmitterList;

        public ParticleFountainComponent(Entity entity, GraphicsDevice device)
            : base(entity)
        {
            this.Device = device;
            this.EmitterList = new List<ParticleEmitter>();
            this.IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        public IReadOnlyList<ParticleEmitter> Emitters => this.EmitterList;

        public void AddEmitter(int count)
        {
            var emitter = new ParticleEmitter(this.Device, count);
            this.EmitterList.Add(emitter);
        }

        public void RemoveEmitter(ParticleEmitter emitter)
            => this.EmitterList.Remove(emitter);

        public void Dispose()
        {
            for (var i = 0; i < this.EmitterList.Count; i++)
            {
                this.EmitterList[i].Dispose();
            }

            this.EmitterList.Clear();
        }
    }
}