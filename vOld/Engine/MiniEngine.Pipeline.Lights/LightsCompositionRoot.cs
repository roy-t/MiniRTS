using LightInject;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Lights.Systems;
using MiniEngine.Pipeline.Lights.Utilities;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Lights
{
    public sealed class LightsCompositionRoot : ICompositionRoot
    {

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<LightPrimitiveLoader>();

            serviceRegistry.Register<IComponentContainer<AmbientLight>, ComponentContainer<AmbientLight>>();
            serviceRegistry.Register<IComponentContainer<DirectionalLight>, ComponentContainer<DirectionalLight>>();
            serviceRegistry.Register<IComponentContainer<PointLight>, ComponentContainer<PointLight>>();            
            serviceRegistry.Register<IComponentContainer<ShadowCastingLight>, ComponentContainer<ShadowCastingLight>>();
            serviceRegistry.Register<IComponentContainer<Sunlight>, ComponentContainer<Sunlight>>();            

            serviceRegistry.Register<LightsFactory>();

            serviceRegistry.Register<AmbientLightFactory>();
            serviceRegistry.Register<DirectionalLightFactory>();
            
            serviceRegistry.Register<PointLightFactory>();
            serviceRegistry.Register<ShadowCastingLightFactory>();
            serviceRegistry.Register<SunlightFactory>();

            serviceRegistry.Register<AmbientLightSystem>();
            serviceRegistry.Register<DirectionalLightSystem>();
            serviceRegistry.Register<PointLightSystem>();
            serviceRegistry.Register<ShadowCastingLightSystem>();
            serviceRegistry.Register<SunlightSystem>();
        }      
    }
}
