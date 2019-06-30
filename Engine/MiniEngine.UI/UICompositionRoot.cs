using LightInject;

namespace MiniEngine.UI
{
    public sealed class UICompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<ImGuiRenderer>();
            serviceRegistry.Register<Editors>();
        }
    }
}
