using System;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Windows
{
    [Service]
    public sealed class WindowService
    {
        public event EventHandler<IWindow>? OpenWindowEvent;

        public void OpenWindow(IWindow window)
            => OpenWindowEvent?.Invoke(this, window);
    }
}
