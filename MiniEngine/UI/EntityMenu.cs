using ImGuiNET;
using MiniEngine.Systems;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MiniEngine.UI
{
    public sealed class EntityMenu
    {
        private readonly EntityManager EntityManager;
        private readonly EntityState State;

        public EntityMenu(UIState ui, EntityManager entityManager)
        {
            this.EntityManager = entityManager;
            this.State = ui.EntityState;
        }        

        public void Render()
        {
            if(ImGui.BeginMenu("Entities"))
            {
                var descriptions = this.EntityManager.Controller.DescribeAllEntities();                
                if (ImGui.MenuItem("Create entity"))
                {
                    var entity = this.EntityManager.Creator.CreateEntity();
                    this.State.SelectedEntity = entity;
                }
                ImGui.Separator();

                var listBoxItem = this.IndexOfEntity(this.State.SelectedEntity, descriptions);                
                if (ImGui.ListBox(string.Empty, ref listBoxItem, descriptions.Select(x => $"{x.Entity} ({x.ComponentCount} components)").ToArray(), descriptions.Count, 20))
                {
                    if (listBoxItem != -1)
                    {
                        this.State.SelectedEntity = descriptions[listBoxItem].Entity;
                        this.State.ShowEntityWindow = true;
                    }
                }

                ImGui.Separator();
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), $"Entities: {descriptions.Count}");
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), $"Components: {descriptions.Sum(x => x.ComponentCount)}");                
                ImGui.EndMenu();
            }
        }

        private int IndexOfEntity(Entity entity, List<EntityDescription> descriptions)
        {
            for (var i = 0; i < descriptions.Count; i++)
            {
                if(descriptions[i].Entity == entity)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
