using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.UI.State;
using MiniEngine.UI.Utilities;

namespace MiniEngine.UI
{
    public sealed class EntityMenu
    {
        private readonly EntityManager EntityManager;
        private readonly EntityState State;
        private readonly List<IComponent> ComponentList;
        private readonly ComponentSearcher ComponentSearcher;

        public EntityMenu(UIState ui, EntityManager entityManager, ComponentSearcher componentSearcher)
        {
            this.EntityManager = entityManager;
            this.State = ui.EntityState;
            this.ComponentList = new List<IComponent>();
            this.ComponentSearcher = componentSearcher;
        }        

        public void Render()
        {
            if(ImGui.BeginMenu("Entities"))
            {
                if (ImGui.MenuItem("Create entity"))
                {
                    var entity = this.EntityManager.Creator.CreateEntity();
                    this.State.SelectedEntity = entity;
                }
                ImGui.Separator();

                var entities = this.EntityManager.Creator.GetAllEntities();
                
                var listBoxItem = this.IndexOfEntity(this.State.SelectedEntity, entities);      
                
                if (ImGui.ListBox(string.Empty, ref listBoxItem, entities.Select(x => $"{x} ({this.GetComponentCount(x)})").ToArray(), entities.Count, 20))
                {
                    if (listBoxItem != -1)
                    {
                        this.State.SelectedEntity = entities[listBoxItem];
                        this.State.ShowEntityWindow = true;
                    }
                }

                ImGui.Separator();
                ImGui.TextColored(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), $"Entities: {entities.Count}");
                ImGui.EndMenu();
            }
        }       

        private int IndexOfEntity(Entity entity, IReadOnlyList<Entity> enties)
        {
            for (var i = 0; i < enties.Count; i++)
            {
                if(enties[i] == entity)
                {
                    return i;
                }
            }

            return -1;
        }       
        

        private int GetComponentCount(Entity entity)
        {
            this.ComponentList.Clear();
            this.ComponentSearcher.GetComponents(entity, this.ComponentList);
            return this.ComponentList.Count;
        }
    }
}
