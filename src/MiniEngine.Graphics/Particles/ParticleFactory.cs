using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Physics;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;


namespace MiniEngine.Graphics.Particles
{
    [Service]
    public sealed class ParticleFactory
    {
        private readonly GraphicsDevice Device;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public ParticleFactory(GraphicsDevice device, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Device = device;
            this.Entities = entities;
            this.Components = components;
        }

        public (ParticleEmitterComponent, TransformComponent, BoundingSphereComponent, ForcesComponent) Create(int paticleCount)
        {
            var entity = this.Entities.Create();

            var particleEmitter = new ParticleEmitterComponent(entity, this.Device, paticleCount);
            var transform = new TransformComponent(entity);
            var bounds = new BoundingSphereComponent(entity, 1.0f); // Automatically adjusted by particle system
            var forces = new ForcesComponent(entity);

            this.Components.Add(particleEmitter, transform, bounds, forces);

            return (particleEmitter, transform, bounds, forces);
        }
    }
}
