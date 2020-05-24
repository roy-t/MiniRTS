using LightInject;
using MiniEngine.GameLogic.Factories;
using MiniEngine.GameLogic.Systems;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Systems.Containers;

namespace MiniEngine.GameLogic
{
    public sealed class GameLogicCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<Accelerometer>, ComponentContainer<Accelerometer>>();

            serviceRegistry.Register<AccelerometerFactory>();

            serviceRegistry.Register<AccelerometerSystem>();
        }

    }
}
