using LightInject;
using MiniEngine.GameLogic.Factories;

namespace MiniEngine.GameLogic
{
    public sealed class GameLogicCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<CarAnimationFactory>();
        }
    }
}
