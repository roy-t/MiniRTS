using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Particles.Systems
{
    public sealed class ParticleSystem : IUpdatableSystem
    {
        private readonly GraphicsDevice Device;
        private readonly WeightedParticlesEffect WeightedParticlesEffect;
        private readonly AverageParticlesEffect AverageParticlesEffect;

        private readonly EntityLinker Linker;
        private readonly List<Emitter> Emitters;
        private readonly List<ParticlePose> Particles;
        private readonly Texture2D NeutralMask;

        private readonly Texture2D NeutralNormalMap;
        private readonly Texture2D NeutralSpecularMap;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly Quad Quad;

        public ParticleSystem(
            GraphicsDevice device,
            WeightedParticlesEffect weightedParticlesEffect,
            AverageParticlesEffect averageParticlesEffect,
            EntityLinker linker,
            Texture2D neutralMask,
            Texture2D neutralNormal,
            Texture2D neutralSpecular)
        {
            this.Device = device;
            this.WeightedParticlesEffect = weightedParticlesEffect;
            this.AverageParticlesEffect = averageParticlesEffect;
            this.Linker = linker;
            this.NeutralMask = neutralMask;
            this.NeutralNormalMap = neutralNormal;
            this.NeutralSpecularMap = neutralSpecular;

            this.Emitters = new List<Emitter>();
            this.Particles = new List<ParticlePose>();

            this.FullScreenTriangle = new FullScreenTriangle();
            this.Quad = new Quad(device);
        }

        public void RenderWeights(IViewPoint viewPoint)
        {
            this.GatherParticles(viewPoint);
            using (this.Device.ParticleState())
            {
                foreach (var particle in this.Particles)
                {
                    this.WeightedParticlesEffect.World = particle.Pose;
                    this.WeightedParticlesEffect.View = viewPoint.View;
                    this.WeightedParticlesEffect.Projection = viewPoint.Projection;
                    this.WeightedParticlesEffect.DiffuseMap = particle.Texture;
                    this.WeightedParticlesEffect.Tint = particle.Tint.ToVector4();                        

                    this.WeightedParticlesEffect.Apply();

                    this.Quad.SetTextureCoordinates(particle.MinUv, particle.MaxUv);
                    this.Quad.Render();
                }
            }
        }

        public void RenderParticles(RenderTarget2D colorMap, RenderTarget2D weightMap)
        {
            using (this.Device.FooState())
            {
                this.AverageParticlesEffect.ColorMap = colorMap;
                this.AverageParticlesEffect.WeightMap = weightMap;

                this.AverageParticlesEffect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Emitters.Clear();
            this.Linker.GetComponents(this.Emitters);
            foreach (var emitter in this.Emitters)
            {
                emitter.Update(elapsed);
            }
        }

        private void GatherParticles(IViewPoint viewPoint)
        {
            this.Particles.Clear();
            this.Emitters.Clear();

            this.Linker.GetComponents(this.Emitters);
            foreach (var emitter in this.Emitters)
            {
                foreach (var particle in emitter.Particles)
                {
                    var particlePose = ComputePose(viewPoint, emitter, particle);
                    this.Particles.Add(particlePose);
                }
            }
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
                Vector3.Distance(particle.Position, viewPoint.Position),
                particle.Tint);
        }

        private static void GetFrame(int frame, int rows, int columns, out Vector2 minUvs, out Vector2 maxUvs)
        {
            var index = frame % (rows * columns);

            var row = index % rows;
            var column = index / columns;

            var width = 1.0f / columns;
            var height = 1.0f / rows;

            minUvs = new Vector2(column * width, row * height);
            maxUvs = new Vector2((column + 1) * width, (row + 1) * height);
        }
    }
}