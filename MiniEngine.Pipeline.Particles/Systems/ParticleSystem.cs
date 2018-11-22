using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Batches;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Effects;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Particles.Systems
{
    public sealed class ParticleSystem : IUpdatableSystem
    {
        private static readonly DistanceComparer PoseComparer = new DistanceComparer();

        private readonly RenderEffect Effect;
        private readonly EntityLinker Linker;
        private readonly List<Emitter> Emitters;
        private readonly Texture2D NeutralMask;
        private readonly Texture2D NeutralNormalMap;
        private readonly Texture2D NeutralSpecularMap;
        private readonly Quad Quad;

        public ParticleSystem(
            GraphicsDevice device,
            RenderEffect effect,
            EntityLinker linker,
            Texture2D neutralMask,
            Texture2D neutralNormal,
            Texture2D neutralSpecular)
        {
            this.Effect = effect;
            this.Linker = linker;
            this.NeutralMask = neutralMask;
            this.NeutralNormalMap = neutralNormal;
            this.NeutralSpecularMap = neutralSpecular;

            this.Emitters = new List<Emitter>();
            this.Quad = new Quad(device);
        }        

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Emitters.Clear();
            this.Linker.GetComponentsOfType(this.Emitters);
            foreach (var emitter in this.Emitters)
            {
                emitter.Update(elapsed);
            }
        }

        public ParticleBatchList ComputeBatches(IViewPoint viewPoint)
        {
            var particles = new List<ParticlePose>();
            
            this.Emitters.Clear();
            this.Linker.GetComponentsOfType(this.Emitters);
            foreach (var emitter in this.Emitters)
            {
                foreach (var particle in emitter.Particles)
                {
                    var particlePose = ComputePose(viewPoint, emitter, particle);
                    particles.Add(particlePose);
                }
            }

            particles.Sort(PoseComparer);

            var particleBatches = new List<ParticleRenderBatch>
            {
                new ParticleRenderBatch(
                this.Quad,
                this.Effect,
                particles,
                viewPoint,
                this.NeutralMask,
                this.NeutralNormalMap,
                this.NeutralSpecularMap)
            };

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
            public int Compare(ParticlePose x, ParticlePose y) => y.Distance.CompareTo(x.Distance);
        }
    }
}