using System.Collections.Generic;
using System.Linq;
using MiniEngine.Configuration;
using MiniEngine.Gui.Tools;
using MiniEngine.Gui.Windows;
using MiniEngine.Systems;

namespace MiniEngine.Editor.Windows
{
    [Service]
    public sealed class SystemsWindow : IWindow
    {
        private readonly Tool ToolSelector;
        private readonly List<ISystem> Systems;

        public SystemsWindow(IEnumerable<ISystem> systems, Tool toolSelector)
        {
            this.Systems = systems.OrderBy(s => s.GetType().Name).ToList();
            this.ToolSelector = toolSelector;
        }

        public string Name => "Systems";
        public bool AllowTransparency => true;

        public void RenderContents()
        {
            Tool.BeginTable("Systems");
            for (var i = 0; i < this.Systems.Count; i++)
            {
                object system = this.Systems[i];
                var type = system.GetType();
                this.ToolSelector.Change(type, ref system, new Property(type.Name));
            }
            Tool.EndTable();
        }
    }
}
