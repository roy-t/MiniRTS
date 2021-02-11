using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Editor.Windows.Converters;
using MiniEngine.Gui;
using MiniEngine.Gui.Tools;
using MiniEngine.Gui.Tools.Generic;
using MiniEngine.Gui.Windows;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;
using Serilog;

namespace MiniEngine.Editor.Windows
{
    [Service]
    public sealed class EntityWindow : IWindow
    {
        private record SerializedState(Entity? SelectedEntity);

        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly Tool ToolSelector;
        private readonly PersistentState<SerializedState> Serializer;

        public EntityWindow(ILogger logger, EntityAdministrator entities, ComponentAdministrator components, Tool toolSelector)
        {
            this.Entities = entities;
            this.Components = components;
            this.ToolSelector = toolSelector;

            this.Serializer = new PersistentState<SerializedState>(logger, new EntityConverter());
        }

        public string Name => "Entities";
        public bool AllowTransparency => true;

        public Entity? SelectedEntity { get; set; }

        public void RenderContents()
        {
            this.ListEntities();
            this.ActOnEntity();

            ImGui.Separator();

            this.ListComponents();
        }

        public void Load(Dictionary<string, string> keyValues)
        {
            if (keyValues.TryGetValue("EntityWindow", out var value))
            {
                var state = this.Serializer.Deserialize(value);
                if (state != null)
                {
                    this.SelectedEntity = state.SelectedEntity;
                }
            }
        }

        public void Save(Dictionary<string, string> keyValues)
        {
            var state = new SerializedState(this.SelectedEntity);
            keyValues["EntityWindow"] = this.Serializer.Serialize(state);
        }

        private void ListComponents()
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
            if (ImGui.Button("Create Entity"))
            {
                this.SelectedEntity = this.Entities.Create();
            }

            if (this.SelectedEntity != null)
            {
                var entity = this.SelectedEntity ?? throw new Exception();
                ImGui.SameLine();
                if (ImGui.Button($"Remove Entity {entity}"))
                {
                    this.Components.MarkForRemoval(entity);
                    this.Entities.Remove(entity);
                    this.SelectedEntity = this.Entities.GetAllEntities().LastOrDefault();
                }
            }
        }

        private void ListEntities()
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
