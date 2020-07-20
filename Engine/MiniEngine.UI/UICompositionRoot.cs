using LightInject;
using MiniEngine.UI.Input;

namespace MiniEngine.UI
{
    public sealed class UICompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<KeyboardInput>();
            serviceRegistry.Register<MouseInput>();

            serviceRegistry.Register<ImGuiRenderer>();
            serviceRegistry.Register<Editors>();
        }
    }
}
