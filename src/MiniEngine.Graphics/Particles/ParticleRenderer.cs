using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems.Components;

namespace MiniEngine.Graphics.Particles
{
    [Service]
    public sealed class ParticleRenderer
    {
        private readonly GraphicsDevice Device;
        private readonly Point Point;
        private readonly IComponentContainer<ParticleEmitterComponent> Components;
        private readonly IComponentContainer<TransformComponent> Transforms;

        public ParticleRenderer(GraphicsDevice device, Point point,
            IComponentContainer<ParticleEmitterComponent> components,
            IComponentContainer<TransformComponent> transforms)
        {
            this.Device = device;
            this.Point = point;
            this.Components = components;
            this.Transforms = transforms;
        }

        public void Draw(Matrix viewProjection, IParticleRendererUser user)
        {
            for (var i = 0; i < this.Components.All.Count; i++)
            {
                var emitter = this.Components.All[i];
                var transform = this.Transforms.Get(emitter.Entity);

                user.ApplyEffect(transform.Matrix * viewProjection, emitter);
                this.Point.RenderInstanced(this.Device, emitter.Instances, emitter.Count);
            }
        }
    }

    public interface IParticleRendererUser
    {
        void ApplyEffect(Matrix worldViewProjection, ParticleEmitterComponent emitter);
    }
}
