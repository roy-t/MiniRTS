using LightInject;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles
{
    public sealed class ParticlesCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<AdditiveEmitterFactory>();
            serviceRegistry.Register<AveragedEmitterFactory>();

            serviceRegistry.Register<AdditiveParticleSystem>();
            serviceRegistry.Register<AveragedParticleSystem>();
        }
    }
}
