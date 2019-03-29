using ImGuiNET;
using MiniEngine.Systems;
using System.Linq;

namespace MiniEngine.UI
{
    public sealed class EntityMenu
    {
        private readonly EntityManager EntityManager;
        private readonly EntityState State;

        private int listBoxItem;

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
                ImGui.Text($"Entities: {descriptions.Count}");
                ImGui.Text($"Components: {descriptions.Sum(x => x.ComponentCount)}");
                ImGui.Separator();

                if (ImGui.ListBox("", ref this.listBoxItem, descriptions.Select(x => $"{x.Entity} ({x.ComponentCount} components)").ToArray(), descriptions.Count, 10))
                {
                    this.State.SelectedEntity = descriptions[this.listBoxItem].Entity;
                    this.State.ShowEntityWindow = true;
                }
                ImGui.EndMenu();
            }
        }
    }
}
