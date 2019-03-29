﻿using ImGuiNET;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using System.Collections.Generic;
using System.Linq;

namespace MiniEngine.UI
{
    public sealed class EntityWindow
    {
        private readonly EntityManager EntityManager;

        public EntityWindow(UIState ui, EntityManager entityManager)
        {
            this.EntityState = ui.EntityState;
            this.EntityManager = entityManager;
        }

        public EntityState EntityState { get; }

        public void Render()
        {
            if (ImGui.Begin($"{this.EntityState.SelectedEntity}", ref this.EntityState.ShowEntityWindow))
            {
                var components = new List<IComponent>();
                this.EntityManager.Linker.GetComponents(this.EntityState.SelectedEntity, components);

                foreach (var component in components)
                {
                    var description = component.Describe();
                    if (ImGui.TreeNode(description.Name + " #" + component.GetHashCode().ToString("00").Substring(0, 2)))
                    {
                        foreach (var property in description.Properties)
                        {
                            Editors.CreateEditor(property.Name, property.Value, property.Min, property.Max, property.Setter);
                        }

                        if (ImGui.Button("Remove Component"))
                        {
                            this.EntityManager.Linker.RemoveComponent(this.EntityState.SelectedEntity, component);
                        }
                        ImGui.TreePop();
                    }
                }

                if (this.EntityManager.Creator.GetAllEntities().Contains(this.EntityState.SelectedEntity))
                {
                    if (ImGui.Button("Destroy Entity"))
                    {
                        this.EntityManager.Controller.DestroyEntity(this.EntityState.SelectedEntity);
                        this.EntityState.ShowEntityWindow = false;
                        var entities = this.EntityManager.Controller.DescribeAllEntities();
                        this.EntityState.SelectedEntity = entities.Any() ? entities.First().Entity : new Entity(-1);
                    }
                }

                ImGui.End();
            }
        }
    }
}
