using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Input;
using MiniEngine.Rendering;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Scenes;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Utilities;
using KeyboardInput = MiniEngine.Input.KeyboardInput;
using MiniEngine.Systems;
using MiniEngine.Telemetry;
using System;
using MiniEngine.UI;
using ImGuiNET;

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
        
        private PerspectiveCamera perspectiveCamera;
        private SpriteBatch spriteBatch;
        private IReadOnlyList<IScene> scenes;
        private IScene currentScene;
        private DebugController debugController;
        private DeferredRenderPipeline renderPipeline;
        private EntityController entityController;
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
            this.gui = new ImGuiRenderer(this);
            this.gui.RebuildFontAtlas();

            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            this.perspectiveCamera = new PerspectiveCamera(this.GraphicsDevice.Viewport);

            this.injector = new Injector(this.GraphicsDevice, this.Content);

            this.keyboardInput = this.injector.Resolve<KeyboardInput>();
            this.mouseInput = this.injector.Resolve<MouseInput>();

            this.entityController = this.injector.Resolve<EntityController>();
            this.debugController = this.injector.Resolve<DebugControllerFactory>().Build(this.perspectiveCamera);
            this.metricServer = this.injector.Resolve<IMetricServer>();
            this.metricServer.Start(7070);

            this.renderPipeline = this.injector.Resolve<DeferredRenderPipeline>();

            this.scenes = this.injector.ResolveAll<IScene>()
                              .ToList()
                              .AsReadOnly();

            foreach (var scene in this.scenes)
            {
                scene.LoadContent(this.Content);
            }

            this.SwitchScenes(this.scenes.First());

            this.ui = UIState.Deserialize(this.renderPipeline.GetGBuffer());
        }

        private void SwitchScenes(IScene scene)
        {
            this.entityController.DestroyAllEntities();
            this.currentScene = scene;
            this.currentScene.Set();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void OnExiting(object sender, EventArgs args) 
            => this.ui.Serialize();

        protected override void Update(GameTime gameTime)
        {
            this.debugController.Update(gameTime.ElapsedGameTime);

            this.currentScene.Update(gameTime.ElapsedGameTime);

            this.keyboardInput.Update();
            this.mouseInput.Update();

            // Do not handle input if game window is not activated
            if (!this.IsActive)
            {
                return;
            }

            if(this.keyboardInput.Click(Keys.F12))
            {
                this.ui.ShowGui = !this.ui.ShowGui;
            }

            // TODO: make these controllers use IMGUI, and always allow the camera to move
            var inputHandled = this.debugController.Update(gameTime.ElapsedGameTime);
            if (inputHandled)
            {
                return;
            }
           
            if (this.keyboardInput.Click(Keys.Escape))
            {
                this.Exit();
            }            

            if (this.keyboardInput.Click(Keys.F))
            {
                this.IsFixedTimeStep = !this.IsFixedTimeStep;
            }           

            if (this.keyboardInput.Click(Keys.Scroll))
            {
                Console.WriteLine(this.entityController.DescribeAllEntities());
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.Window.Title = $"{gameTime.ElapsedGameTime.TotalMilliseconds:F2}ms, {1.0f / gameTime.ElapsedGameTime.TotalSeconds:F2} fps, Fixed Time Step: {this.IsFixedTimeStep} (press 'F' so switch). Input State: {this.debugController.DescribeState()}";
            this.Window.Title +=
                $" camera ({this.perspectiveCamera.Position.X:F2}, {this.perspectiveCamera.Position.Y:F2}, {this.perspectiveCamera.Position.Z:F2})";

            var result = this.renderPipeline.Render(this.perspectiveCamera, (float)gameTime.ElapsedGameTime.TotalSeconds);

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

                        if (ImGui.BeginMenu("Debug"))
                        {
                            if (ImGui.MenuItem(DebugDisplay.None.ToString(), null, this.ui.DebugDisplay == DebugDisplay.None))
                            {
                                this.ui.DebugDisplay = DebugDisplay.None;
                            }

                            if (ImGui.BeginMenu(DebugDisplay.Combined.ToString()))
                            {
                                var descriptions = this.renderPipeline.GetGBuffer().RenderTargets;
                                foreach (var target in descriptions)
                                {
                                    var selected = this.ui.SelectedRenderTargets.Contains(target);
                                    if (ImGui.MenuItem(target.Name, null, selected))
                                    {
                                        if (selected)
                                        {
                                            this.ui.SelectedRenderTargets.Remove(target);
                                        }
                                        else
                                        {
                                            this.ui.SelectedRenderTargets.Add(target);
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

                            var showDemo = this.ui.ShowDemo;
                            if (ImGui.MenuItem("Show Demo Window", null, ref showDemo))
                            {
                                this.ui.ShowDemo = showDemo;
                            }                            

                            ImGui.EndMenu();
                        }

                        ImGui.EndMainMenuBar();
                    }                    

                    if (this.ui.ShowDemo) { ImGui.ShowDemoWindow(); }
                }
                this.gui.EndLayout();
            }

            base.Draw(gameTime);
        }
    }
}

