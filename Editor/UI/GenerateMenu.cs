using ImGuiNET;
using MiniEngine.Generators;
using MiniEngine.Primitives.Cameras;
using MiniEngine.UI.State;

namespace MiniEngine.UI
{
    public sealed class GenerateMenu : IMenu
    {
        private readonly TurretGenerator TurretGenerator;

        public GenerateMenu(TurretGenerator turretGenerator)
        {
            this.TurretGenerator = turretGenerator;
        }

        public UIState State { get; set; }

        public void Render(PerspectiveCamera camera)
        {
            if(ImGui.BeginMenu("Generate"))
            {
                if(ImGui.MenuItem("Turret"))
                {
                    var turret = this.TurretGenerator.Generate(camera.Position);
                    this.State.EntityState.SelectedEntity = turret.Entity;
                    this.State.EntityState.ShowEntityWindow = true;
                }

                ImGui.EndMenu();
            }

            
        }
    }
}
