using System;
using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Gui.Tools;
using MiniEngine.Gui.Tools.Generic;
using MiniEngine.Gui.Windows;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Windows
{
    [Service]
    public sealed class EntityWindow : IWindow
    {
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly Tool ToolSelector;

        public EntityWindow(EntityAdministrator entities, ComponentAdministrator components, Tool toolSelector)
        {
            this.Entities = entities;
            this.Components = components;
            this.ToolSelector = toolSelector;
        }

        public string Name => "Entities";
        public bool AllowTransparency => true;

        public Entity? SelectedEntity { get; set; }

        public void RenderContents()
        {
            this.SelectEntity();
            this.ActOnEntity();
            ImGui.Separator();
            this.ShowEntityComponents();
        }

        private void ShowEntityComponents()
        {
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
        }

        private void ActOnEntity()
        {
            if (this.SelectedEntity != null)
            {
                var entity = this.SelectedEntity ?? throw new System.Exception();
                if (ImGui.Button($"Remove Entity {entity}"))
                {
                    this.Components.MarkForRemoval(entity);
                    this.Entities.Remove(entity);
                    this.SelectedEntity = null;
                }
            }
        }

        private void SelectEntity()
        {
            var entities = this.Entities.GetAllEntities().ToArray();
            var index = Array.IndexOf(entities, this.SelectedEntity);
            var entityNames = entities.Select(e => $"{e.Id}").ToArray();
            if (ImGui.ListBox("Entities", ref index, entityNames, entityNames.Length))
            {
                this.SelectedEntity = entities[index];
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
    }
}
