using LightInject;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Models.Systems;

namespace MiniEngine.Pipeline.Models
{
    public sealed class ModelsCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<OpaqueModelFactory>();
            serviceRegistry.Register<TransparentModelFactory>();

            serviceRegistry.Register<ModelSystem>();
        }
    }
}
