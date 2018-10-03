using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Systems;

namespace MiniEngine.Rendering.Systems
{
    public sealed class ParticleSystem : ISystem
    {
        private static readonly Random Random = new Random();

        private readonly GraphicsDevice Device;
        private readonly RenderEffect Effect;
        private readonly Texture2D NullMask;
        private readonly Texture2D NullNormalMap;
        private readonly Texture2D NullSpecularMap;

        private readonly Dictionary<Entity, Emitter> Emitters;

        public ParticleSystem(
            GraphicsDevice device,
            RenderEffect effect,
            Texture2D nullMask,
            Texture2D nullNormal,
            Texture2D nullSpecular)
        {
            this.Device = device;
            this.Effect = effect;
            this.NullMask = nullMask;
            this.NullNormalMap = nullNormal;
            this.NullSpecularMap = nullSpecular;
            this.Emitters = new Dictionary<Entity, Emitter>();
        }

        public void Add(Entity entity, Vector3 position, Texture2D texture, int rows, int columns)
        {
            this.Emitters.Add(entity, new Emitter(position, texture, rows, columns));
        }

        public bool Contains(Entity entity) => this.Emitters.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var emitter = this.Emitters[entity];            
            return $"emitter, position: {emitter.Position}, particles: {emitter.Particles.Count}";
        }

        public void Remove(Entity entity) => this.Emitters.Remove(entity);

        public ParticleBatchList ComputeBatches(PerspectiveCamera camera)
        {
            var particles = new List<ParticlePose>();

            foreach (var emitter in this.Emitters.Values)
            {
                foreach (var particle in emitter.Particles)
                {
                    var particlePose = UpdateParticle(emitter, particle);
                    particles.Add(particlePose);
                }
            }

            var particleRenderBatch = new ParticleRenderBatch(
                this.Effect,
                particles,
                camera,
                this.NullMask,
                this.NullNormalMap,
                this.NullSpecularMap);
            var particleBatches = new List<ParticleRenderBatch> {particleRenderBatch};
            return new ParticleBatchList(particleBatches);
        }

        private ParticlePose UpdateParticle(Emitter emitter, Particle particle)
        {
            // TODO: do something with time and stuff
            // TODO: move the quad to the particle render batch and just tell it which uv coordinates to use
            var quad = new Quad(this.Device);
            return new ParticlePose(quad, emitter.Texture, Matrix.CreateScale(100));            
        }
    }
}
