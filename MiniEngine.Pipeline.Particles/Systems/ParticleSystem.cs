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
        private readonly AdditiveParticlesEffect AdditiveParticlesEffect;

        private readonly EntityLinker Linker;
        private readonly List<AveragedEmitter> AveragedEmitters;
        private readonly List<AdditiveEmitter> AdditiveEmitters;
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
            AdditiveParticlesEffect additiveParticlesEffect,
            EntityLinker linker,
            Texture2D neutralMask,
            Texture2D neutralNormal,
            Texture2D neutralSpecular)
        {
            this.Device = device;

            this.WeightedParticlesEffect = weightedParticlesEffect;
            this.AverageParticlesEffect = averageParticlesEffect;
            this.AdditiveParticlesEffect = additiveParticlesEffect;

            this.Linker = linker;
            this.NeutralMask = neutralMask;
            this.NeutralNormalMap = neutralNormal;
            this.NeutralSpecularMap = neutralSpecular;

            this.AveragedEmitters = new List<AveragedEmitter>();
            this.AdditiveEmitters = new List<AdditiveEmitter>();

            this.Particles = new List<ParticlePose>();

            this.FullScreenTriangle = new FullScreenTriangle();
            this.Quad = new Quad(device);
        }

        public void RenderParticleWeights(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Particles.Clear();
            this.GatherParticles(this.AveragedEmitters, camera);

            using (this.Device.WeightedParticlesState())
            {
                foreach (var particle in this.Particles)
                {
                    this.WeightedParticlesEffect.World = particle.Pose;
                    this.WeightedParticlesEffect.View = camera.View;
                    this.WeightedParticlesEffect.Projection = camera.Projection;
                    this.WeightedParticlesEffect.DiffuseMap = particle.Texture;
                    this.WeightedParticlesEffect.Tint = particle.Tint.ToVector4();
                    this.WeightedParticlesEffect.DepthMap = gBuffer.DepthTarget;
                    this.WeightedParticlesEffect.InverseViewProjection = camera.InverseViewProjection;

                    this.WeightedParticlesEffect.Apply();

                    this.Quad.SetTextureCoordinates(particle.MinUv, particle.MaxUv);
                    this.Quad.Render();
                }
            }
        }

        public void AverageParticles(RenderTarget2D colorMap, RenderTarget2D weightMap)
        {
            using (this.Device.AdditiveParticleState())
            {
                this.AverageParticlesEffect.ColorMap = colorMap;
                this.AverageParticlesEffect.WeightMap = weightMap;

                this.AverageParticlesEffect.Apply();

                this.FullScreenTriangle.Render(this.Device);

            }           
        }

        public void RenderAdditiveParticles(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Particles.Clear();
            this.GatherParticles(this.AdditiveEmitters, camera);

            using (this.Device.AdditiveParticleState())
            {
                foreach (var particle in this.Particles)
                {
                    this.AdditiveParticlesEffect.World = particle.Pose;
                    this.AdditiveParticlesEffect.View = camera.View;
                    this.AdditiveParticlesEffect.Projection = camera.Projection;
                    this.AdditiveParticlesEffect.DiffuseMap = particle.Texture;
                    this.AdditiveParticlesEffect.Tint = particle.Tint.ToVector4();
                    this.AdditiveParticlesEffect.DepthMap = gBuffer.DepthTarget;
                    this.AdditiveParticlesEffect.InverseViewProjection = camera.InverseViewProjection;

                    this.AdditiveParticlesEffect.Apply();

                    this.Quad.SetTextureCoordinates(particle.MinUv, particle.MaxUv);
                    this.Quad.Render();
                }
            }
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Update(this.AveragedEmitters, elapsed);
            this.Update(this.AdditiveEmitters, elapsed);            
        }

        private void Update<T>(List<T> emitters, Seconds elapsed)
            where T : AEmitter
        {
            emitters.Clear();
            this.Linker.GetComponents(emitters);
            foreach (var emitter in emitters)
            {
                emitter.Update(elapsed);
            }
        }

        private void GatherParticles<T>(List<T> emitters, IViewPoint viewPoint)
            where T : AEmitter
        {
            emitters.Clear();                        
            this.Linker.GetComponents(emitters);            

            foreach (var emitter in emitters)
            {
                foreach (var particle in emitter.Particles)
                {
                    var particlePose = ComputePose(viewPoint, emitter, particle);
                    this.Particles.Add(particlePose);
                }
            }
        }

        private static ParticlePose ComputePose(IViewPoint viewPoint, AEmitter emitter, Particle particle)
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