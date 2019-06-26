using LightInject;
using MiniEngine.Pipeline.Shadows.Factories;
using MiniEngine.Pipeline.Shadows.Systems;

namespace MiniEngine.Pipeline.Shadows
{
    public sealed class ShadowsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<CascadedShadowMapFactory>();
            serviceRegistry.Register<ShadowMapFactory>();

            serviceRegistry.Register<CascadedShadowMapSystem>();
            serviceRegistry.Register<ShadowMapSystem>();       
        }
    }
}

