using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Gui;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Workspaces.Editors
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

        public void Draw()
        {
            this.RenderEntities();
            this.RenderComponents();
        }

        private void RenderComponents()
        {
            if (ImGui.Begin("Components"))
            {
                ImGui.Text($"Selected: {this.selectedEntity?.ToString()}");
                if (this.selectedEntity.HasValue)
                {
                    var components = this.Components.GetComponents(this.selectedEntity.Value);
                    foreach (var component in components)
                    {
                        this.ComponentEditor.DrawComponent(component);
                        ImGui.Separator();
                    }
                }
                ImGui.End();
            }
        }

        private void RenderEntities()
        {
            if (ImGui.Begin("Entities"))
            {
                var entities = this.Entities.GetAllEntities();
                var entityNames = entities.Select(e => $"{e.Id}").ToArray();
                ImGui.ListBox("Entities", ref this.entityIndex, entityNames, entityNames.Length);


                if (this.entityIndex < entities.Count)
                {
                    this.selectedEntity = entities[this.entityIndex];
                }
                else
                {
                    this.selectedEntity = null;
                }

                ImGui.End();
            }
        }
    }
}
