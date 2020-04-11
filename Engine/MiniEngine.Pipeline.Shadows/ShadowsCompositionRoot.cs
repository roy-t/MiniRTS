using LightInject;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Pipeline.Shadows.Factories;
using MiniEngine.Pipeline.Shadows.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Shadows
{
    public sealed class ShadowsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<CascadedShadowMap>, ComponentContainer<CascadedShadowMap>>();
            serviceRegistry.Register<IComponentContainer<ShadowMap>, ComponentContainer<ShadowMap>>();            

            serviceRegistry.Register<CascadedShadowMapFactory>();
            serviceRegistry.Register<ShadowMapFactory>();

            serviceRegistry.Register<CascadedShadowMapSystem>();
            serviceRegistry.Register<ShadowMapSystem>();       
        }
    }
}
