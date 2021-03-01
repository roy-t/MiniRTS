using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Particles
{
    public abstract class AFountainComponent : AComponent, IDisposable
    {
        private readonly GraphicsDevice Device;
        private readonly List<ParticleEmitter> EmitterList;

        public AFountainComponent(Entity entity, GraphicsDevice device)
            : base(entity)
        {
            this.Device = device;
            this.EmitterList = new List<ParticleEmitter>();
            this.IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        public IReadOnlyList<ParticleEmitter> Emitters => this.EmitterList;

        public void AddEmitter(Texture2D texture, IParticleSpawnFunction spawnFunction, IParticleUpdateFunction updateFunction)
        {
            var emitter = new ParticleEmitter(new ParticleBuffer(this.Device), texture, spawnFunction, updateFunction);
            this.EmitterList.Add(emitter);
        }

        public void RemoveEmitter(ParticleEmitter emitter)
            => this.EmitterList.Remove(emitter);

        public void Update(float elapsed, Matrix transform)
        {
            for (var i = 0; i < this.EmitterList.Count; i++)
            {
                var emitter = this.EmitterList[i];

                emitter.RemoveOldParticles(elapsed);

                if (this.IsEnabled)
                {
                    emitter.SpawnNewParticles(elapsed, transform);
                }

                emitter.UpdateParticles(elapsed, transform);
            }
        }

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