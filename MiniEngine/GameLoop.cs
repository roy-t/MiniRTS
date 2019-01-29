using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering;
using MiniEngine.Scenes;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Telemetry;
using MiniEngine.UI;
using MiniEngine.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using KeyboardInput = MiniEngine.Input.KeyboardInput;

namespace MiniEngine
{
    public sealed class GameLoop : Game
    {        
        private readonly GraphicsDeviceManager Graphics;        
        private Injector injector;

        private UIState ui;
        private KeyboardInput keyboardInput;
        private MouseInput mouseInput;            
        private ImGuiRenderer gui;
        
        private PerspectiveCamera camera;
        private SpriteBatch spriteBatch;
        private IReadOnlyList<IScene> scenes;
        private IScene currentScene;
        private CameraController cameraController;
        private LightsController lightsController;
        private OutlineFactory outlineFactory;
        private DeferredRenderPipeline renderPipeline;
        private EntityCreator entityCreator;
        private EntityController entityController;
        private EntityLinker entityLinker;
        private IMetricServer metricServer;

        public GameLoop()
        {
            this.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.ui = new UIState();
        }

        protected override void LoadContent()
        {            
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.camera = new PerspectiveCamera(this.GraphicsDevice.Viewport);

            this.injector = new Injector(this.GraphicsDevice, this.Content);
            this.keyboardInput = this.injector.Resolve<KeyboardInput>();
            this.mouseInput = this.injector.Resolve<MouseInput>();
            this.entityCreator = this.injector.Resolve<EntityCreator>();
            this.entityController = this.injector.Resolve<EntityController>();
            this.entityLinker = this.injector.Resolve<EntityLinker>();
            this.outlineFactory = this.injector.Resolve<OutlineFactory>();

            this.cameraController = new CameraController(this.keyboardInput, this.mouseInput, this.camera);
            this.lightsController = new LightsController(this.entityCreator, this.entityController, this.entityLinker, this.injector.Resolve<LightsFactory>());
            
            
            this.renderPipeline = this.injector.Resolve<DeferredRenderPipeline>();

            this.scenes = this.injector.ResolveAll<IScene>()
                              .ToList()
                              .AsReadOnly();

            foreach (var scene in this.scenes)
            {
                scene.LoadContent(this.Content);
            }

            this.SwitchScenes(this.scenes.First());
            
            this.gui = new ImGuiRenderer(this);
            this.gui.RebuildFontAtlas();
            this.ui = UIState.Deserialize(this.renderPipeline.GetGBuffer());

            this.camera.Move(this.ui.CameraPosition, this.ui.CameraLookAt);
            this.metricServer = this.injector.Resolve<IMetricServer>();
            this.metricServer.Start(7070);
        }

        private void SwitchScenes(IScene scene)
        {
            this.entityController.DestroyAllEntities();
            this.currentScene = scene;
            this.currentScene.Set();
        }

        protected override void OnExiting(object sender, EventArgs args) 
            => this.ui.Serialize(this.camera.Position, this.camera.LookAt);

        protected override void Update(GameTime gameTime)
        {
            var elapsed = (Seconds)gameTime.ElapsedGameTime;
            this.currentScene.Update(elapsed);
            // Do not handle input if game window is not activated
            if (!this.IsActive)
            {
                return;
            }

            this.keyboardInput.Update();
            this.mouseInput.Update();


            this.cameraController.Update(elapsed);                                 

            if(this.keyboardInput.Click(Keys.F12))
            {
                this.ui.ShowGui = !this.ui.ShowGui;
            }
            
            if (this.keyboardInput.Click(Keys.Escape))
            {
                this.Exit();
            }            

            if (this.keyboardInput.Click(Keys.Scroll))
            {
                Console.WriteLine(this.entityController.DescribeAllEntities());
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.Window.Title = $"{gameTime.ElapsedGameTime.TotalMilliseconds:F2}ms, {1.0f / gameTime.ElapsedGameTime.TotalSeconds:F2} fps.";
            this.Window.Title +=
                $" Camera ({this.camera.Position.X:F2}, {this.camera.Position.Y:F2}, {this.camera.Position.Z:F2})";

            var result = this.renderPipeline.Render(this.camera, (float)gameTime.ElapsedGameTime.TotalSeconds);

            this.GraphicsDevice.SetRenderTarget(null);
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);


            this.spriteBatch.Draw(
                result,
                new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height),
                null,
                Color.White);

            var gBuffer = this.renderPipeline.GetGBuffer();

            switch (this.ui.DebugDisplay)
            {                
                case DebugDisplay.None:
                    break;
                case DebugDisplay.Single:
                    this.spriteBatch.Draw(
                           this.ui.SelectedRenderTarget.RenderTarget,
                           new Vector2(0, 0),
                           null,
                           Color.White,
                           0.0f,
                           Vector2.Zero,
                           1.0f,
                           SpriteEffects.None,
                           0);                    
                    break;
                case DebugDisplay.Combined:
                    if (this.ui.SelectedRenderTargets.Any())
                    {
                        var count = this.ui.SelectedRenderTargets.Count;
                        var xStep = this.GraphicsDevice.Viewport.Width / this.ui.Columns;
                        var yStep = xStep / this.GraphicsDevice.Viewport.AspectRatio;
                        for (var i = 0; i < this.ui.SelectedRenderTargets.Count; i++)
                        {                            
                            this.spriteBatch.Draw(
                                this.ui.SelectedRenderTargets[i].RenderTarget,
                                new Vector2(xStep * (i % this.ui.Columns), yStep * (i / this.ui.Columns)),
                                null,
                                Color.White,
                                0.0f,
                                Vector2.Zero,
                                1.0f / this.ui.Columns,
                                SpriteEffects.None,
                                0);
                        }
                    }
                    break;
            }

            this.spriteBatch.End();

            // TODO: make the menu cleaner, separate into one file per menu, store data in one object per menu
            // create helper methods for most common cases (like enabling graphics). Try to store outside of
            // main project if possible. Try to auto generate a lot based on factories and stuff.

            if (this.ui.ShowGui)
            {
                this.gui.BeginLayout(gameTime);
                {
                    if (ImGui.BeginMainMenuBar())
                    {
                        if (ImGui.BeginMenu("File"))
                        {
                            if (ImGui.BeginMenu("Scenes"))
                            {
                                foreach (var scene in this.scenes)
                                {
                                    if (ImGui.MenuItem(scene.Name, null, scene == this.currentScene))
                                    {
                                        this.SwitchScenes(scene);
                                    }
                                }
                                ImGui.EndMenu();
                            }

                            if (ImGui.MenuItem("Hide GUI", "F12")) { this.ui.ShowGui = false; }
                            if (ImGui.MenuItem("Quit")) { this.Exit(); }
                            ImGui.EndMenu();
                        }

                        if (ImGui.BeginMenu("Entities"))
                        {
                            var descriptions = this.entityController.DescribeAllEntities();
                            ImGui.Text($"Entities: {descriptions.Count}");
                            ImGui.Text($"Components: {descriptions.Sum(x => x.ComponentCount)}");
                            ImGui.Separator();
                            
                            if(ImGui.ListBox("", ref this.ui.ListBoxItem, descriptions.Select(x => $"{x.Entity} ({x.ComponentCount} components)").ToArray(), descriptions.Count, 10))
                            {
                                this.ui.SelectedEntity = descriptions[this.ui.ListBoxItem].Entity;
                                this.ui.ShowEntityWindow = true;
                            }
                            ImGui.EndMenu();
                        }

                        if (ImGui.BeginMenu("Create"))
                        {
                            if (ImGui.MenuItem("Ambient Light"))
                            {
                                var entity = this.lightsController.CreateAmbientLight();
                                this.ui.SelectedEntity = entity;
                                this.ui.ShowEntityWindow = true;
                            }
                            if (ImGui.MenuItem("Directional Light"))
                            {
                                var entity = this.lightsController.CreateDirectionalLight(this.camera.Position, this.camera.LookAt);
                                this.ui.SelectedEntity = entity;
                                this.ui.ShowEntityWindow = true;
                            }
                            if (ImGui.MenuItem("Point Light"))
                            {
                                var entity = this.lightsController.CreatePointLight(this.camera.Position);
                                this.ui.SelectedEntity = entity;
                                this.ui.ShowEntityWindow = true;
                            }
                            if (ImGui.MenuItem("Shadow Casting Light"))
                            {
                                var entity = this.lightsController.CreateShadowCastingLight(this.camera.Position, this.camera.LookAt);
                                this.ui.SelectedEntity = entity;
                                this.ui.ShowEntityWindow = true;
                            }
                            if (ImGui.MenuItem("Sun Light"))
                            {
                                var entity = this.lightsController.CreateSunLight(this.camera.Position, this.camera.LookAt);
                                this.ui.SelectedEntity = entity;
                                this.ui.ShowEntityWindow = true;
                            }                                                        
                            if (ImGui.MenuItem("Remove created lights"))
                            {
                                this.lightsController.RemoveCreatedLights();
                            }
                            if (ImGui.MenuItem("Remove all lights"))
                            {
                                this.lightsController.RemoveAllLights();
                            }
                            ImGui.Separator();
                            var enableOutline = this.entityLinker.HasComponent<AModel>(this.ui.SelectedEntity) && !this.entityLinker.HasComponent<Outline>(this.ui.SelectedEntity);
                            if(ImGui.MenuItem("Outline", enableOutline))
                            {
                                this.outlineFactory.Construct(this.ui.SelectedEntity);
                            }
                            ImGui.EndMenu();
                        }

                        if(ImGui.BeginMenu("Rendering"))
                        {
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.EnableModels), this.renderPipeline.Settings.EnableModels, false, true, x => { this.renderPipeline.Settings.EnableModels = (bool)x; this.renderPipeline.Recreate(); });
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.EnableParticles), this.renderPipeline.Settings.EnableParticles, false, true, x => { this.renderPipeline.Settings.EnableParticles = (bool)x; this.renderPipeline.Recreate(); });
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.EnableShadows), this.renderPipeline.Settings.EnableShadows, false, true, x => { this.renderPipeline.Settings.EnableShadows = (bool)x; this.renderPipeline.Recreate(); });                            
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.Enable2DOutlines), this.renderPipeline.Settings.Enable2DOutlines, false, true, x => { this.renderPipeline.Settings.Enable2DOutlines = (bool)x; this.renderPipeline.Recreate(); });                            
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.Enable3DOutlines), this.renderPipeline.Settings.Enable3DOutlines, false, true, x => { this.renderPipeline.Settings.Enable3DOutlines = (bool)x; this.renderPipeline.Recreate(); });                            
                            ImGui.Separator();
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.ModelSettings.FxaaFactor), this.renderPipeline.Settings.ModelSettings.FxaaFactor, 0, 16, x => { this.renderPipeline.Settings.ModelSettings.FxaaFactor = (int)x; this.renderPipeline.Recreate(); });                            
                            ImGui.Separator();
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.LightSettings.EnableAmbientLights), this.renderPipeline.Settings.LightSettings.EnableAmbientLights, false, true, x => { this.renderPipeline.Settings.LightSettings.EnableAmbientLights = (bool)x; this.renderPipeline.Recreate(); });                            
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.LightSettings.EnableDirectionalLights), this.renderPipeline.Settings.LightSettings.EnableDirectionalLights, false, true, x => { this.renderPipeline.Settings.LightSettings.EnableDirectionalLights = (bool)x; this.renderPipeline.Recreate(); });                            
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.LightSettings.EnablePointLights), this.renderPipeline.Settings.LightSettings.EnablePointLights, false, true, x => { this.renderPipeline.Settings.LightSettings.EnablePointLights = (bool)x; this.renderPipeline.Recreate(); });                            
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.LightSettings.EnableShadowCastingLights), this.renderPipeline.Settings.LightSettings.EnableShadowCastingLights, false, true, x => { this.renderPipeline.Settings.LightSettings.EnableShadowCastingLights = (bool)x; this.renderPipeline.Recreate(); });                            
                            Editors.CreateEditor(nameof(this.renderPipeline.Settings.LightSettings.EnableSunLights), this.renderPipeline.Settings.LightSettings.EnableSunLights, false, true, x => { this.renderPipeline.Settings.LightSettings.EnableSunLights = (bool)x; this.renderPipeline.Recreate(); });                                                        

                            ImGui.EndMenu();
                        }
                        
                        if (ImGui.BeginMenu("Debug"))
                        {
                            if (ImGui.MenuItem(DebugDisplay.None.ToString(), null, this.ui.DebugDisplay == DebugDisplay.None))
                            {
                                this.ui.DebugDisplay = DebugDisplay.None;
                            }

                            if (ImGui.BeginMenu(DebugDisplay.Combined.ToString()))
                            {
                                var descriptions = this.renderPipeline.GetGBuffer().RenderTargets;
                                var columns = this.ui.Columns;
                                if(ImGui.SliderInt("Columns", ref columns, 1, Math.Max(5, descriptions.Count)))
                                {
                                    this.ui.Columns = columns;
                                }

                                ImGui.Separator();

                                foreach (var target in descriptions)
                                {
                                    var selected = this.ui.SelectedRenderTargets.Contains(target);
                                    if (ImGui.Checkbox(target.Name, ref selected))
                                    {
                                        if (selected)
                                        {
                                            this.ui.SelectedRenderTargets.Add(target);
                                        }
                                        else
                                        {
                                            this.ui.SelectedRenderTargets.Remove(target);
                                            
                                        }

                                        this.ui.SelectedRenderTargets.Sort();
                                        this.ui.DebugDisplay = DebugDisplay.Combined;
                                    }
                                }

                                ImGui.EndMenu();
                            }

                            if (ImGui.BeginMenu(DebugDisplay.Single.ToString()))
                            {
                                var descriptions = this.renderPipeline.GetGBuffer().RenderTargets;
                                foreach (var target in descriptions)
                                {
                                    var selected = this.ui.SelectedRenderTarget == target;
                                    if (ImGui.MenuItem(target.Name, null, selected))
                                    {
                                        this.ui.SelectedRenderTarget = target;
                                        this.ui.DebugDisplay = DebugDisplay.Single;
                                    }
                                }
                                ImGui.EndMenu();
                            }

                            if (ImGui.MenuItem("Fixed Timestep", null, this.IsFixedTimeStep))
                            {
                                this.IsFixedTimeStep = !this.IsFixedTimeStep;
                            }

                            var showDemo = this.ui.ShowDemo;
                            if (ImGui.MenuItem("Show Demo Window", null, ref showDemo))
                            {
                                this.ui.ShowDemo = showDemo;
                            }

                            ImGui.EndMenu();
                        }

                        ImGui.EndMainMenuBar();
                    }                    
              
                    if (this.ui.ShowEntityWindow)
                    {
                        if (ImGui.Begin($"{this.ui.SelectedEntity}", ref this.ui.ShowEntityWindow))
                        {
                            var components = new List<IComponent>();
                            this.entityLinker.GetComponents(this.ui.SelectedEntity, components);

                            foreach (var component in components)
                            {
                                var description = component.Describe();
                                if (ImGui.TreeNode(description.Name + " #" + component.GetHashCode().ToString("00").Substring(0, 2)))
                                {
                                    foreach (var property in description.Properties)
                                    {
                                        Editors.CreateEditor(property.Name, property.Value, property.Min, property.Max, property.Setter);
                                    }

                                    if (ImGui.Button("Remove Component"))
                                    {
                                        this.entityLinker.RemoveComponent(this.ui.SelectedEntity, component);                                       
                                    }
                                    ImGui.TreePop();
                                }

                               
                            }
                        }
                        if (this.entityCreator.GetAllEntities().Contains(this.ui.SelectedEntity))
                        {
                            if (ImGui.Button("Destroy Entity"))
                            {
                                this.entityController.DestroyEntity(this.ui.SelectedEntity);
                                this.ui.ShowEntityWindow = false;
                                var entities = this.entityController.DescribeAllEntities();
                                this.ui.SelectedEntity = entities.Any() ? entities.First().Entity : new Entity(-1);
                            }
                        }
                        ImGui.End();
                    }

                    if (this.ui.ShowDemo) { ImGui.ShowDemoWindow(); }
                }
                this.gui.EndLayout();
            }
            base.Draw(gameTime);
        }
    }
}

