using LightInject;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Systems
{
    public sealed class SystemsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<EntityController>();

            serviceRegistry.Register<IComponentContainer<Parent>, ComponentContainer<Parent>>();
            serviceRegistry.Register<ParentFactory>();
        }
    }
}
