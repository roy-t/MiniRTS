using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Controllers;
using MiniEngine.CutScene;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.UI.State;

namespace MiniEngine.UI
{
    public sealed class CreateMenu
    {
        private readonly EntityManager EntityManager;
        private readonly DebugInfoFactory OutlineFactory;
        private readonly ProjectorFactory ProjectorFactory;
        private readonly Texture2D Texture;
        private readonly LightsController LightsController;
        private readonly WaypointFactory WaypointFactory;
        private readonly PerspectiveCamera Camera;

        public CreateMenu(UIState ui, EntityManager entityManager, DebugInfoFactory outLineFactory, WaypointFactory waypointFactory,
            ProjectorFactory projectorFactory, Texture2D texture, LightsController lightsController, PerspectiveCamera camera)
        {
            this.EntityManager = entityManager;
            this.OutlineFactory = outLineFactory;
            this.WaypointFactory = waypointFactory;
            this.ProjectorFactory = projectorFactory;
            this.Texture = texture;
            this.LightsController = lightsController;
            this.Camera = camera;
            this.State = ui.EntityState;
        }

        public EntityState State { get; }

        public void Render()
        {
            if (ImGui.BeginMenu("Create"))
            {
                if (ImGui.MenuItem("Ambient Light"))
                {
                    var entity = this.LightsController.CreateAmbientLight();
                    this.State.SelectedEntity = entity;
                    this.State.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Directional Light"))
                {
                    var entity = this.LightsController.CreateDirectionalLight(this.Camera.Position, this.Camera.LookAt);
                    this.State.SelectedEntity = entity;
                    this.State.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Point Light"))
                {
                    var entity = this.LightsController.CreatePointLight(this.Camera.Position);
                    this.State.SelectedEntity = entity;
                    this.State.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Shadow Casting Light"))
                {
                    var entity = this.LightsController.CreateShadowCastingLight(this.Camera.Position, this.Camera.LookAt);
                    this.State.SelectedEntity = entity;
                    this.State.ShowEntityWindow = true;
                }
                if (ImGui.MenuItem("Sun Light"))
                {
                    var entity = this.LightsController.CreateSunLight(this.Camera.Position, this.Camera.LookAt);
                    this.State.SelectedEntity = entity;
                    this.State.ShowEntityWindow = true;
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
                    this.ProjectorFactory.Construct(this.State.SelectedEntity, this.Texture, Color.White * 0.5f, this.Camera.Position, this.Camera.LookAt);                    
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Debug info"))
                {
                    this.OutlineFactory.Construct(this.State.SelectedEntity);
                }

                ImGui.Separator();

                if(ImGui.MenuItem("Waypoint"))
                {
                    this.WaypointFactory.Construct(this.State.SelectedEntity, 1.0f, this.Camera.Position, this.Camera.Position + (this.Camera.Forward * 100));
                }

                ImGui.EndMenu();
            }
        }
    }
}
