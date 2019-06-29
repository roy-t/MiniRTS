using LightInject;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Debug.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Debug
{
    public sealed class DebugCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<DebugInfo>, ComponentList<DebugInfo>>();

            serviceRegistry.Register<DebugInfoFactory>();

            serviceRegistry.Register<BoundarySystem>();
            serviceRegistry.Register<IconSystem>();
        }
    }
}

