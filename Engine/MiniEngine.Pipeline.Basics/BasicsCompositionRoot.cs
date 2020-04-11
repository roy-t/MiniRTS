using LightInject;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Basics
{
    public class BasicsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<Pose>, ComponentContainer<Pose>>();

            serviceRegistry.Register<PoseFactory>();
        }
    }
}
