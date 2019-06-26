using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.UI.State;

namespace MiniEngine.UI
{
    public sealed class EntityMenu
    {
        private readonly EntityManager EntityManager;
        private readonly EntityState State;
        private readonly List<IComponent> Components;

        public EntityMenu(UIState ui, EntityManager entityManager)
        {
            this.EntityManager = entityManager;
            this.State = ui.EntityState;
            this.Components = new List<IComponent>();
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
                if (ImGui.ListBox(string.Empty, ref listBoxItem, entities.Select(x => $"{x} ({this.ComponentCount(x)} components)").ToArray(), entities.Count, 20))
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

        private int ComponentCount(Entity entity)
        {
            this.Components.Clear();
            this.EntityManager.Linker.GetComponents(entity, this.Components);

            return this.Components.Count;
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
    }
}
