using LightInject;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Models.Generators;
using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Models
{
    public sealed class ModelsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<OpaqueModel>, ComponentContainer<OpaqueModel>>();
            serviceRegistry.Register<IComponentContainer<TransparentModel>, ComponentContainer<TransparentModel>>();
            serviceRegistry.Register<IComponentContainer<AAnimation>, ComponentContainer<AAnimation>>();
            serviceRegistry.Register<IComponentContainer<UVAnimation>, ComponentContainer<UVAnimation>>();
            serviceRegistry.Register<IComponentContainer<Geometry>, ComponentContainer<Geometry>>();

            serviceRegistry.Register<OpaqueModelFactory>();
            serviceRegistry.Register<TransparentModelFactory>();
            serviceRegistry.Register<UVAnimationFactory>();
            serviceRegistry.Register<GeometryFactory>();

            serviceRegistry.Register<ModelSystem>();
            serviceRegistry.Register<AnimationSystem>();
            serviceRegistry.Register<UVAnimationSystem>();
            serviceRegistry.Register<GeometrySystem>();


            serviceRegistry.Register<SpherifiedCubeGenerator>();
        }
    }
}
