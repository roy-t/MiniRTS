using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.UI.State;
using MiniEngine.UI.Utilities;

namespace MiniEngine.UI
{
    public sealed class EntityMenu : IMenu
    {
        private readonly EntityController EntityController;
        
        private readonly List<IComponent> ComponentList;
        private readonly ComponentSearcher ComponentSearcher;

        public EntityMenu(EntityController entityController, ComponentSearcher componentSearcher)
        {
            this.EntityController = entityController;            
            this.ComponentList = new List<IComponent>();
            this.ComponentSearcher = componentSearcher;

            this.State = new UIState();
        }        

        public UIState State { get; set; }
        public EntityState EntityState => this.State.EntityState;

        public void Render(PerspectiveCamera camera)
        {
            if(ImGui.BeginMenu("Entities"))
            {
                if (ImGui.MenuItem("Create entity"))
                {
                    var entity = this.EntityController.CreateEntity();
                    this.EntityState.SelectedEntity = entity;
                }
                ImGui.Separator();

                var entities = this.EntityController.GetAllEntities();
                
                var listBoxItem = this.IndexOfEntity(this.EntityState.SelectedEntity, entities);      
                
                if (ImGui.ListBox(string.Empty, ref listBoxItem, entities.Select(x => $"{x} ({this.GetComponentCount(x)})").ToArray(), entities.Count, 20))
                {
                    if (listBoxItem != -1)
                    {
                        this.EntityState.SelectedEntity = entities[listBoxItem];
                        this.EntityState.ShowEntityWindow = true;
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
