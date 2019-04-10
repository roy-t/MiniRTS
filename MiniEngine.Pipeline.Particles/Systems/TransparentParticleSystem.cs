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
    public class TransparentParticleSystem : IUpdatableSystem
    {
        private readonly GraphicsDevice Device;
        private readonly WeightedParticlesEffect WeightedParticlesEffect;
        private readonly AverageParticlesEffect AverageParticlesEffect;

        private readonly EntityLinker Linker;
        private readonly List<AveragedEmitter> Emitters;
        private readonly List<ParticlePose> Particles;

        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly FullScreenQuad Quad;

        public TransparentParticleSystem(
            GraphicsDevice device,
            WeightedParticlesEffect weightedParticlesEffect,
            AverageParticlesEffect averageParticlesEffect,
            EntityLinker linker)
        {
            this.Device = device;

            this.WeightedParticlesEffect = weightedParticlesEffect;
            this.AverageParticlesEffect = averageParticlesEffect;

            this.Linker = linker;

            this.Emitters = new List<AveragedEmitter>();

            this.Particles = new List<ParticlePose>();

            this.FullScreenTriangle = new FullScreenTriangle();
            this.Quad = new FullScreenQuad(device);
        }

        public void RenderParticleWeights(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Particles.Clear();
            this.Emitters.Clear();
            this.Linker.GetComponents(this.Emitters);

            ParticleHelper.GatherParticles(this.Emitters, camera, this.Particles);

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
      
        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Emitters.Clear();
            this.Linker.GetComponents(this.Emitters);

            foreach (var emitter in this.Emitters)
            {
                emitter.Update(elapsed);
            }
        }        
    }
}