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
using MiniEngine.Utilities.Extensions;

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
        private readonly Quad Quad;

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
            this.Quad = new Quad(device);
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
                    var particlePose = UpdateParticle(camera, emitter, particle);
                    particles.Add(particlePose);
                }
            }

            var particleRenderBatch = new ParticleRenderBatch(
                this.Quad,
                this.Effect,
                particles,
                camera,
                this.NullMask,
                this.NullNormalMap,
                this.NullSpecularMap);
            var particleBatches = new List<ParticleRenderBatch> {particleRenderBatch};
            return new ParticleBatchList(particleBatches);
        }

        private static int counter = 0;
        private ParticlePose UpdateParticle(PerspectiveCamera camera, Emitter emitter, Particle particle)
        {
            var forward = Vector3.Normalize(camera.LookAt - camera.Position);
            var matrix = Matrix.CreateScale(10)
                         * Matrix.CreateBillboard(particle.Position, camera.Position, Vector3.Up, forward);
                         

            counter++;

            var index = counter / 60;

            GetFrame(index, emitter.Rows, emitter.Columns, out var minUvs, out var maxUvs);

            // TODO: do something with time and stuff            
            return new ParticlePose(minUvs, maxUvs, emitter.Texture, matrix);            
        }

        private void GetFrame(int frame, int rows, int columns, out Vector2 minUvs, out Vector2 maxUvs)
        {
            var row = frame / rows;
            var column = frame % rows;

            var width = 1.0f / columns;
            var height = 1.0f / rows;

            minUvs = new Vector2(column * width, row * height);
            maxUvs = new Vector2((column + 1) * width, (row + 1) * height);
        }
    }
}
