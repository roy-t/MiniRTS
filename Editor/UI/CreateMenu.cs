using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Controllers;
using MiniEngine.CutScene;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.UI.State;

namespace MiniEngine.UI
{
    public sealed class CreateMenu : IMenu
    {
        private readonly DebugInfoFactory OutlineFactory;
        private readonly ProjectorFactory ProjectorFactory;
        private readonly Texture2D Texture;
        private readonly LightsController LightsController;
        private readonly WaypointFactory WaypointFactory;

        public CreateMenu(DebugInfoFactory outLineFactory, WaypointFactory waypointFactory,
            ProjectorFactory projectorFactory, ContentManager content, LightsController lightsController)
        {
            this.OutlineFactory = outLineFactory;
            this.WaypointFactory = waypointFactory;
            this.ProjectorFactory = projectorFactory;
            this.Texture = content.Load<Texture2D>("Debug");
            this.LightsController = lightsController;
            this.State = new UIState();
        }

        public UIState State { get; set; }
        public EntityState EntityState => this.State.EntityState;

        public void Render(PerspectiveCamera camera)
        {
            if (ImGui.BeginMenu("Create"))
            {
                if (ImGui.MenuItem("Ambient Light"))
                {
                    var entity = this.LightsController.CreateAmbientLight();
                    this.EntityState.SelectedEntity = entity;
                    this.EntityState.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Directional Light"))
                {
                    var entity = this.LightsController.CreateDirectionalLight(camera.Position, camera.LookAt);
                    this.EntityState.SelectedEntity = entity;
                    this.EntityState.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Point Light"))
                {
                    var entity = this.LightsController.CreatePointLight(camera.Position);
                    this.EntityState.SelectedEntity = entity;
                    this.EntityState.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Shadow Casting Light"))
                {
                    var entity = this.LightsController.CreateShadowCastingLight(camera.Position, camera.LookAt);
                    this.EntityState.SelectedEntity = entity;
                    this.EntityState.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Sun Light"))
                {
                    var entity = this.LightsController.CreateSunLight(camera.Position, camera.LookAt);
                    this.EntityState.SelectedEntity = entity;
                    this.EntityState.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Remove created lights"))
                {
                    this.LightsController.RemoveCreatedLights();
                }
                if (ImGui.MenuItem("Remove all lights"))
                {
                    this.LightsController.RemoveAllLights();
                }

                ImGui.Separator();                               

                if (ImGui.MenuItem("Projector"))
                {
                    this.ProjectorFactory.Construct(this.EntityState.SelectedEntity, this.Texture, Color.White * 0.5f, camera.Position, camera.LookAt);                    
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Debug info"))
                {
                    this.OutlineFactory.Construct(this.EntityState.SelectedEntity);
                }

                ImGui.Separator();

                if(ImGui.MenuItem("Waypoint"))
                {
                    this.WaypointFactory.Construct(this.EntityState.SelectedEntity, 1.0f, camera.Position, camera.Position + (camera.Forward * 100));
                }

                ImGui.EndMenu();
            }
        }
    }
}
