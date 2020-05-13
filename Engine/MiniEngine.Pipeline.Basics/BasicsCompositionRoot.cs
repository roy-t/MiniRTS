using LightInject;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Basics.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Basics
{
    public class BasicsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<Pose>, ComponentContainer<Pose>>();
            serviceRegistry.Register<IComponentContainer<Bounds>, ComponentContainer<Bounds>>();
            serviceRegistry.Register<IComponentContainer<Offset>, ComponentContainer<Offset>>();

            serviceRegistry.Register<PoseFactory>();
            serviceRegistry.Register<OffsetFactory>();

            serviceRegistry.Register<BoundsSystem>();
            serviceRegistry.Register<OffsetSystem>();
        }
    }
}
