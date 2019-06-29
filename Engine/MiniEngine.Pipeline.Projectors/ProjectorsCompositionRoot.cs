using LightInject;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Pipeline.Projectors.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Projectors
{
    public sealed class ProjectorsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<DynamicTexture>, ComponentList<DynamicTexture>>();
            serviceRegistry.Register<IComponentContainer<Projector>, ComponentList<Projector>>();

            serviceRegistry.Register<DynamicTextureFactory>();
            serviceRegistry.Register<ProjectorFactory>();

            serviceRegistry.Register<DynamicTextureSystem>();
            serviceRegistry.Register<ProjectorSystem>();                      
        }
    }
}
