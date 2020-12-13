using MiniEngine.Primitives.Cameras;
using MiniEngine.UI.State;

namespace MiniEngine.UI
{
    public interface IMenu
    {
        UIState State { get; set; }
        void Render(PerspectiveCamera camera);
    }
}
