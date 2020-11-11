using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Gui;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Editors
{
    [Service]
    public sealed class EntityEditor
    {
        private readonly ComponentEditor ComponentEditor;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private Entity? selectedEntity;
        private int entityIndex;

        public EntityEditor(ComponentEditor componentEditor, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.ComponentEditor = componentEditor;
            this.Entities = entities;
            this.Components = components;
        }

        public bool ShowEntityWindow = true;
        public bool ShowComponentWindow = true;

        public void Draw()
        {
            if (this.ShowEntityWindow)
            {
                ImGui.Begin("Entities", ref this.ShowEntityWindow);
                var entities = this.Entities.Copy();
                var entityNames = entities.Select(e => $"{e.Id}").ToArray();
                ImGui.ListBox("Entities", ref this.entityIndex, entityNames, entityNames.Length);

                this.selectedEntity = entities[this.entityIndex];

                ImGui.End();
            }

            if (this.ShowComponentWindow)
            {
                ImGui.Begin("Components", ref this.ShowComponentWindow);
                ImGui.Text($"Selected: {this.selectedEntity?.ToString()}");
                if (this.selectedEntity.HasValue)
                {
                    var components = this.Components.GetComponents(this.selectedEntity.Value);
                    //var componentNames = components.Select(c => c.GetType().Name).ToArray();
                    //ImGui.ListBox("Components", ref this.componentIndex, componentNames, componentNames.Length);

                    foreach (var component in components)
                    {
                        this.ComponentEditor.DrawComponent(component);
                        ImGui.Separator();
                    }
                }
            }
        }
    }
}
