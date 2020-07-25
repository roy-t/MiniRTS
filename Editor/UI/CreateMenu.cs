using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Controllers;
using MiniEngine.CutScene;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.UI.State;
using MiniEngine.UI.Utilities;

namespace MiniEngine.UI
{
    public sealed class CreateMenu : IMenu
    {
        private readonly DebugInfoFactory DebugInfoFactory;
        private readonly ProjectorFactory ProjectorFactory;
        private readonly Texture2D Texture;
        private readonly LightsController LightsController;
        private readonly WaypointFactory WaypointFactory;
        private readonly ComponentSearcher ComponentSearcher;
        private readonly List<IComponent> Components;


        public CreateMenu(DebugInfoFactory outLineFactory, WaypointFactory waypointFactory,
            ProjectorFactory projectorFactory, ContentManager content, LightsController lightsController, ComponentSearcher componentSearcher)
        {
            this.DebugInfoFactory = outLineFactory;
            this.WaypointFactory = waypointFactory;
            this.ProjectorFactory = projectorFactory;
            this.Texture = content.Load<Texture2D>("Debug");
            this.LightsController = lightsController;
            this.ComponentSearcher = componentSearcher;

            this.State = new UIState();
            this.Components = new List<IComponent>();
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

                ImGui.Separator();

                if (ImGui.MenuItem("Projector"))
                {
                    this.ProjectorFactory.Construct(this.EntityState.SelectedEntity, this.Texture, Color.White * 0.5f, camera.Position, camera.LookAt);
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Debug info"))
                {
                    this.Components.Clear();
                    this.ComponentSearcher.GetComponents(this.EntityState.SelectedEntity, this.Components);

                    for (var i = 0; i < this.Components.Count; i++)
                    {
                        if (this.Components[i] is Bounds)
                        {
                            this.DebugInfoFactory.Construct(this.EntityState.SelectedEntity, IconType.Model);
                        }
                    }
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Waypoint"))
                {
                    this.WaypointFactory.Construct(this.EntityState.SelectedEntity, 1.0f, camera.Position, camera.Position + (camera.Forward * 100));
                }

                ImGui.EndMenu();
            }
        }
    }
}
