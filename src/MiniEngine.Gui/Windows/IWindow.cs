using System.Collections.Generic;

namespace MiniEngine.Gui.Windows
{
    public interface IWindow
    {
        string Name { get; }

        bool AllowTransparency { get; }

        void RenderContents();

        void Load(Dictionary<string, string> keyValues) { }

        void Save(Dictionary<string, string> keyValues) { }
    }
}
