using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.UI
{
    public sealed class EntityWindow
    {
        private readonly Editors Editors;
        private readonly EntityState EntityState;
        private readonly EntityManager EntityManager;

        private readonly Dictionary<Type, int> ComponentCounter;
        
        
        public EntityWindow(Editors editors, UIState ui, EntityManager entityManager)
        {
            this.Editors = editors;
            this.EntityState = ui.EntityState;            
            this.EntityManager = entityManager;

            this.ComponentCounter = new Dictionary<Type, int>();
        }
        
        public void Render()
        {
            if (ImGui.Begin("Entity Details", ref this.EntityState.ShowEntityWindow, ImGuiWindowFlags.NoCollapse))
            {
                ImGui.Text($"{this.EntityState.SelectedEntity}");

                this.ComponentCounter.Clear();

                var components = new List<IComponent>();
                this.EntityManager.Linker.GetComponents(this.EntityState.SelectedEntity, components);

                foreach (var component in components)
                {
                    // ImGui requires a unique name for every node, so for each component we add
                    // check how many of that component we've already added and use that in the name
                    var count = this.Count(component);

                    var description = component.Describe();
                    if (ImGui.TreeNode(description.Name + " #" + count.ToString("00")))
                    {
                        foreach (var property in description.Properties)
                        {
                            this.Editors.Create(property.Name, property.Value, property.MinMax, property.Setter);
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

        private int Count(IComponent component)
        {
            if (this.ComponentCounter.ContainsKey(component.GetType()))
            {
                this.ComponentCounter[component.GetType()] += 1;
            }
            else
            {
                this.ComponentCounter.Add(component.GetType(), 0);
            }

            var id = this.ComponentCounter[component.GetType()];
            return id;
        }
    }
}
