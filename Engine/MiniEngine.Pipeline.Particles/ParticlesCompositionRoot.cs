using LightInject;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Pipeline.Particles.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Particles
{
    public sealed class ParticlesCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<AdditiveEmitter>, ComponentList<AdditiveEmitter>>();
            serviceRegistry.Register<IComponentContainer<AveragedEmitter>, ComponentList<AveragedEmitter>>();

            serviceRegistry.Register<AdditiveEmitterFactory>();
            serviceRegistry.Register<AveragedEmitterFactory>();

            serviceRegistry.Register<AdditiveParticleSystem>();
            serviceRegistry.Register<AveragedParticleSystem>();
        }
    }
}
