using LightInject;

namespace MiniEngine.Net
{
    public sealed class NetCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<NetworkLogger>();
            serviceRegistry.Register<Server>();
            serviceRegistry.Register<Client>();
        }
    }
}
