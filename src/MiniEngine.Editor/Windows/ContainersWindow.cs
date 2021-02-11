using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Gui.Tools;
using MiniEngine.Gui.Windows;
using MiniEngine.Systems.Components;

namespace MiniEngine.Editor.Windows
{
    [Service]
    public sealed class ContainersWindow : IWindow
    {
        private readonly IReadOnlyList<IComponentContainer> Containers;
        private readonly Tool ToolSelector;

        public ContainersWindow(ContainerStore containerStore, Tool toolSelector)
        {
            this.Containers = containerStore.GetAllContainers().OrderBy(c => c.ComponentType.Name).ToList();
            this.ToolSelector = toolSelector;
        }

        public string Name => "Containers";
        public bool AllowTransparency => true;

        public void RenderContents()
        {
            ImGui.Text($"{this.Containers.Select(c => c.Count).Sum()} components in {this.Containers.Count} containers");

            Tool.BeginTable("Containers");
            for (var i = 0; i < this.Containers.Count; i++)
            {
                object container = this.Containers[i];
                var targetType = this.Containers[i].ComponentType;
                var type = container.GetType();
                this.ToolSelector.Change(type, ref container, new Property($"{targetType.Name}s [{this.Containers[i].Count}]", $"{type.Name}.{targetType.Name}"));
            }
            Tool.EndTable();
        }
    }
}
