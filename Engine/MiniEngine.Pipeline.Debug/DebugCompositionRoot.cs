using LightInject;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug
{
    public sealed class DebugCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<DebugInfoFactory>();

            serviceRegistry.Register<BoundarySystem>();
            serviceRegistry.Register<IconSystem>();
        }
    }
}

