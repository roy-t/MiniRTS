using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Gui.Tools;
using MiniEngine.Gui.Tools.Generic;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Workspaces.Editors
{
    [Service]
    public sealed class EntityEditor
    {
        private readonly Tool ToolSelector;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public EntityEditor(Tool toolSelector, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.ToolSelector = toolSelector;
            this.Entities = entities;
            this.Components = components;
        }

        public void Draw()
        {
            this.RenderEntities();
            this.RenderComponents();
        }
        public Entity? SelectedEntity { get; set; }

        private void RenderComponents()
        {
            if (ImGui.Begin("Components"))
            {
                ImGui.Text($"Selected entity: {this.SelectedEntity?.ToString()}");
                ImGui.Checkbox("Show readonly properties", ref ObjectTool.ShowReadOnlyProperties);
                Tool.BeginTable("components");
                if (this.SelectedEntity.HasValue)
                {
                    var components = this.Components.GetComponents(this.SelectedEntity.Value);
                    foreach (var component in components)
                    {
                        this.DrawComponent(component);
                    }
                }
                Tool.EndTable();

                ImGui.End();
            }
        }

        private void DrawComponent(AComponent component)
        {
            var type = component.GetType();
            var changed = this.ToolSelector.Change(ref component, new Property(type.Name));
            if (changed)
            {
                component.ChangeState.Change();
            }
        }

        private void RenderEntities()
        {
            if (ImGui.Begin("Entities"))
            {
                var entities = this.Entities.GetAllEntities();
                var index = GetIndex(entities, this.SelectedEntity);
                var entityNames = entities.Select(e => $"{e.Id}").ToArray();
                ImGui.ListBox("Entities", ref index, entityNames, entityNames.Length);

                if (index >= 0 && index < entities.Count)
                {
                    this.SelectedEntity = entities[index];
                }
                else
                {
                    this.SelectedEntity = null;
                }

                ImGui.End();
            }
        }

        private static int GetIndex(IReadOnlyList<Entity> entities, Entity? entity)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i] == entity)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
