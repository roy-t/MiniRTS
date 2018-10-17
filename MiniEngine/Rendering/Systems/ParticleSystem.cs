using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Systems
{
    public sealed class ParticleSystem : IUpdatableSystem
    {
        private static readonly DistanceComparer PoseComparer = new DistanceComparer();

        private readonly RenderEffect Effect;

        private readonly Dictionary<Entity, Emitter> Emitters;
        private readonly Texture2D NeutralMask;
        private readonly Texture2D NeutralNormalMap;
        private readonly Texture2D NeutralSpecularMap;
        private readonly Quad Quad;

        public ParticleSystem(
            GraphicsDevice device,
            RenderEffect effect,
            Texture2D neutralMask,
            Texture2D neutralNormal,
            Texture2D neutralSpecular)
        {
            this.Effect = effect;
            this.NeutralMask = neutralMask;
            this.NeutralNormalMap = neutralNormal;
            this.NeutralSpecularMap = neutralSpecular;

            this.Emitters = new Dictionary<Entity, Emitter>();
            this.Quad = new Quad(device);
        }

        public bool Contains(Entity entity)
        {
            return this.Emitters.ContainsKey(entity);
        }

        public string Describe(Entity entity)
        {
            var emitter = this.Emitters[entity];
            return $"emitter, position: {emitter.Position}, particles: {emitter.Particles.Count}";
        }

        public void Remove(Entity entity)
        {
            this.Emitters.Remove(entity);
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            foreach (var emitter in this.Emitters.Values)
                emitter.Update(elapsed);
        }

        public void Add(Entity entity, Vector3 position, Texture2D texture, int rows, int columns)
        {
            this.Emitters.Add(entity, new Emitter(position, texture, rows, columns));
        }

        public ParticleBatchList ComputeBatches(IViewPoint viewPoint)
        {
            var particles = new List<ParticlePose>();

            foreach (var emitter in this.Emitters.Values)
            {
                foreach (var particle in emitter.Particles)
                {
                    var particlePose = ComputePose(viewPoint, emitter, particle);
                    particles.Add(particlePose);
                }
            }

            particles.Sort(PoseComparer);

            var particleRenderBatch = new ParticleRenderBatch(
                this.Quad,
                this.Effect,
                particles,
                viewPoint,
                this.NeutralMask,
                this.NeutralNormalMap,
                this.NeutralSpecularMap);
            var particleBatches = new List<ParticleRenderBatch> { particleRenderBatch };
            return new ParticleBatchList(particleBatches);
        }

        private static ParticlePose ComputePose(IViewPoint viewPoint, Emitter emitter, Particle particle)
        {
            var matrix = Matrix.CreateScale(particle.Scale)
                         * Matrix.CreateBillboard(particle.Position, viewPoint.Position, Vector3.Up, viewPoint.Forward);


            GetFrame(particle.Frame, emitter.Rows, emitter.Columns, out var minUvs, out var maxUvs);
            return new ParticlePose(
                minUvs,
                maxUvs,
                emitter.Texture,
                matrix,
                Vector3.Distance(particle.Position, viewPoint.Position));
        }

        private static void GetFrame(int frame, int rows, int columns, out Vector2 minUvs, out Vector2 maxUvs)
        {
            var row = frame / rows;
            var column = frame % rows;

            var width = 1.0f / columns;
            var height = 1.0f / rows;

            minUvs = new Vector2(column * width, row * height);
            maxUvs = new Vector2((column + 1) * width, (row + 1) * height);
        }

        private class DistanceComparer : IComparer<ParticlePose>
        {
            public int Compare(ParticlePose x, ParticlePose y)
            {
                return y.Distance.CompareTo(x.Distance);
            }
        }
    }
}