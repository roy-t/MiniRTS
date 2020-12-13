using LightInject;

namespace MiniEngine.Effects
{
    public sealed class EffectCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry) => serviceRegistry.Register<EffectFactory>();
    }
}
