using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Controllers;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;

namespace MiniEngine.UI
{
    public sealed class CreateMenu
    {
        private readonly EntityManager EntityManager;
        private readonly OutlineFactory OutlineFactory;
        private readonly ProjectorFactory ProjectorFactory;
        private readonly Texture2D Texture;
        private readonly LightsController LightsController;
        private readonly PerspectiveCamera Camera;

        public CreateMenu(UIState ui, EntityManager entityManager, OutlineFactory outLineFactory,
            ProjectorFactory projectorFactory, Texture2D texture, LightsController lightsController, PerspectiveCamera camera)
        {
            this.EntityManager = entityManager;
            this.OutlineFactory = outLineFactory;
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

                var enableOutline = this.EntityManager.Linker.HasComponent<AModel>(this.State.SelectedEntity) && !this.EntityManager.Linker.HasComponent<Outline>(this.State.SelectedEntity);
                if (enableOutline)
                {
                    if (ImGui.MenuItem("Outline", enableOutline))
                    {
                        this.OutlineFactory.Construct(this.State.SelectedEntity);
                    }

                    ImGui.Separator();
                }

                if (ImGui.MenuItem("Projector"))
                {
                    this.ProjectorFactory.Construct(this.State.SelectedEntity, this.Texture, this.Camera.Position, this.Camera.LookAt);                    
                }

                ImGui.EndMenu();
            }
        }
    }
}
