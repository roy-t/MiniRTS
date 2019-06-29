using LightInject;

namespace MiniEngine.Systems
{
    public sealed class SystemsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<EntityController>();
        }
    }
}
