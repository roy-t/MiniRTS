using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Pipeline.Lights.Factories;
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
        
        private CameraController cameraController;
        private LightsController lightsController;
        private OutlineFactory outlineFactory;
        private DeferredRenderPipeline renderPipeline;
        private EntityCreator entityCreator;
        private EntityController entityController;
        private EntityLinker entityLinker;
        private IMetricServer metricServer;

        private UIRenderer uiRenderer;
        private FileMenu fileMenu;
        private EntityMenu entitiesMenu;        
        private CreateMenu createMenu;
        private RenderingMenu renderingMenu;
        private DebugMenu debugMenu;

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

        public IReadOnlyList<IScene> Scenes { get; private set; }
        public IScene CurrentScene { get; private set; }

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

            this.Scenes = this.injector.ResolveAll<IScene>()
                              .ToList()
                              .AsReadOnly();

            foreach (var scene in this.Scenes)
            {
                scene.LoadContent(this.Content);
            }

            this.SwitchScenes(this.Scenes.First());
            

            this.gui = new ImGuiRenderer(this);
            this.gui.RebuildFontAtlas();

            this.ui = UIState.Deserialize(this.renderPipeline.GetGBuffer());

            this.uiRenderer = new UIRenderer(this.renderPipeline);

            this.fileMenu = new FileMenu(this.ui, this);            
            this.entitiesMenu = new EntityMenu(this.ui, this.entityController);            
            this.createMenu = new CreateMenu(this.ui, this.entityLinker, this.outlineFactory, this.lightsController, this.camera);            
            this.debugMenu = new DebugMenu(this.ui, this.uiRenderer, this);            
            this.renderingMenu = new RenderingMenu(this.ui, this.renderPipeline);

            this.camera.Move(this.ui.EditorState.CameraPosition, this.ui.EditorState.CameraLookAt);
            this.metricServer = this.injector.Resolve<IMetricServer>();
            this.metricServer.Start(7070);
        }

        public void SwitchScenes(IScene scene)
        {
            this.entityController.DestroyAllEntities();
            this.CurrentScene = scene;
            this.CurrentScene.Set();
        }

        protected override void OnExiting(object sender, EventArgs args) 
            => this.ui.Serialize(this.camera.Position, this.camera.LookAt);

        protected override void Update(GameTime gameTime)
        {
            var elapsed = (Seconds)gameTime.ElapsedGameTime;
            this.CurrentScene.Update(elapsed);
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
                this.ui.EditorState.ShowGui = !this.ui.EditorState.ShowGui;
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

            switch (this.ui.DebugState.DebugDisplay)
            {                
                case DebugDisplay.None:
                    break;
                case DebugDisplay.Single:
                    this.spriteBatch.Draw(
                        this.uiRenderer.GetRenderTarget(this.ui.DebugState.SelectedRenderTarget),
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
                    if (this.ui.DebugState.SelectedRenderTargets.Any())
                    {
                        var count = this.ui.DebugState.SelectedRenderTargets.Count;
                        var xStep = this.GraphicsDevice.Viewport.Width / this.ui.DebugState.Columns;
                        var yStep = xStep / this.GraphicsDevice.Viewport.AspectRatio;
                        for (var i = 0; i < this.ui.DebugState.SelectedRenderTargets.Count; i++)
                        {                            
                            this.spriteBatch.Draw(
                                this.uiRenderer.GetRenderTarget(this.ui.DebugState.SelectedRenderTargets[i]),
                                new Vector2(xStep * (i % this.ui.DebugState.Columns), yStep * (i / this.ui.DebugState.Columns)),
                                null,
                                Color.White,
                                0.0f,
                                Vector2.Zero,
                                1.0f / this.ui.DebugState.Columns,
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

            if (this.ui.EditorState.ShowGui)
            {
                this.gui.BeginLayout(gameTime);
                {
                    if (ImGui.BeginMainMenuBar())
                    {
                        this.fileMenu.Render();
                        this.entitiesMenu.Render();
                        this.createMenu.Render();
                        this.renderingMenu.Render();
                        this.debugMenu.Render();
                                               
                        ImGui.EndMainMenuBar();
                    }                    
              
                    if (this.ui.EntityState.ShowEntityWindow)
                    {
                        if (ImGui.Begin($"{this.ui.EntityState.SelectedEntity}", ref this.ui.EntityState.ShowEntityWindow))
                        {
                            var components = new List<IComponent>();
                            this.entityLinker.GetComponents(this.ui.EntityState.SelectedEntity, components);

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
                                        this.entityLinker.RemoveComponent(this.ui.EntityState.SelectedEntity, component);                                       
                                    }
                                    ImGui.TreePop();
                                }

                               
                            }
                        }
                        if (this.entityCreator.GetAllEntities().Contains(this.ui.EntityState.SelectedEntity))
                        {
                            if (ImGui.Button("Destroy Entity"))
                            {
                                this.entityController.DestroyEntity(this.ui.EntityState.SelectedEntity);
                                this.ui.EntityState.ShowEntityWindow = false;
                                var entities = this.entityController.DescribeAllEntities();
                                this.ui.EntityState.SelectedEntity = entities.Any() ? entities.First().Entity : new Entity(-1);
                            }
                        }
                        ImGui.End();
                    }

                    if (this.ui.DebugState.ShowDemo) { ImGui.ShowDemoWindow(); }
                }
                this.gui.EndLayout();
            }
            base.Draw(gameTime);
        }
    }
}

