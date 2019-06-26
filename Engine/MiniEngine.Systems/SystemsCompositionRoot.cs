using LightInject;

namespace MiniEngine.Systems
{
    public sealed class SystemsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<EntityCreator>();
            serviceRegistry.Register<EntityLinker>();
            serviceRegistry.Register<EntityController>();
            serviceRegistry.Register<EntityManager>();
        }
    }
}
