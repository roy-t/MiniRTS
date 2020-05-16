using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Particles.Systems
{
    public class AveragedParticleSystem : IUpdatableSystem
    {
        private readonly GraphicsDevice Device;
        private readonly WeightedParticlesEffect WeightedParticlesEffect;
        private readonly AverageParticlesEffect AverageParticlesEffect;

        private readonly IComponentContainer<AveragedEmitter> Emitters;
        private readonly IComponentContainer<Pose> Poses;
        private readonly List<ParticlePose> Particles;

        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly UnitQuad Quad;

        public AveragedParticleSystem(
            GraphicsDevice device,
            EffectFactory effectFactory,
            IComponentContainer<AveragedEmitter> emitters,
            IComponentContainer<Pose> poses)
        {
            this.Device = device;
            this.WeightedParticlesEffect = effectFactory.Construct<WeightedParticlesEffect>();
            this.AverageParticlesEffect = effectFactory.Construct<AverageParticlesEffect>();
            this.Emitters = emitters;
            this.Poses = poses;

            this.Particles = new List<ParticlePose>();

            this.FullScreenTriangle = new FullScreenTriangle();
            this.Quad = new UnitQuad(device);
        }

        public void RenderParticleWeights(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Particles.Clear();

            ParticleHelper.GatherParticles(this.Emitters, camera, this.Particles);

            this.Device.WeightedParticlesState();

            for (var i = 0; i < this.Particles.Count; i++)
            {
                var particle = this.Particles[i];

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

        public void AverageParticles(RenderTarget2D colorMap, RenderTarget2D weightMap)
        {
            this.Device.AdditiveParticleState();

            this.AverageParticlesEffect.ColorMap = colorMap;
            this.AverageParticlesEffect.WeightMap = weightMap;

            this.AverageParticlesEffect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            for (var i = 0; i < this.Emitters.Count; i++)
            {
                var emitter = this.Emitters[i];
                var pose = this.Poses.Get(emitter.Entity);
                emitter.Update(elapsed, pose);
            }
        }
    }
}