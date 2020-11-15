using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using MiniEngine.Editor.Controllers;
using MiniEngine.Editor.Workspaces;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.Skybox;
using MiniEngine.Graphics.Utilities;
using MiniEngine.Gui;
using MiniEngine.SceneManagement;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;
using MiniEngine.Systems.Pipeline;

namespace MiniEngine.Editor
{
    [Service]
    internal sealed class GameLoop
    {
        private readonly GraphicsDeviceManager Graphics;
        private readonly WorkspaceManager WorkspaceManager;
        private readonly GraphicsDevice Device;
        private readonly SpriteBatch SpriteBatch;
        private readonly GameTimer GameTimer;
        private readonly GameWindow Window;
        private readonly ContentStack Content;
        private readonly FrameService FrameService;
        private readonly ImGuiRenderer Gui;
        private readonly CubeMapGenerator CubeMapGenerator;
        private readonly IrradianceMapGenerator IrradianceMapGenerator;
        private readonly EnvironmentMapGenerator EnvironmentMapGenerator;
        private readonly BrdfLutGenerator BrdfLutGenerator;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly KeyboardController Keyboard;
        private readonly MouseController Mouse;
        private readonly CameraController CameraController;
        private readonly Editors.EntityEditor EntityEditor;
        private readonly FrameCounter FrameCounter;

        private readonly string[] SkyboxNames;
        private readonly TextureCube[] SkyboxTextures;
        private readonly TextureCube[] IrradianceTextures;
        private readonly TextureCube[] EnvironmentTextures;
        private int currentSkyboxTexture = 0;

        private readonly ParallelPipeline RenderPipeline;

        private bool renderUi = true;

        public GameLoop(GraphicsDeviceManager graphics, WorkspaceManager workspace, GraphicsDevice device, SpriteBatch spriteBatch, GameTimer gameTimer, GameWindow window, ContentStack content, FrameService frameService, ImGuiRenderer imGui, CubeMapGenerator cubeMapGenerator, IrradianceMapGenerator irradianceMapGenerator, EnvironmentMapGenerator environmentMapGenerator, BrdfLutGenerator brdfLutGenerator, EntityAdministrator entities, ComponentAdministrator components, RenderPipelineBuilder renderPipelineBuilder, KeyboardController keyboard, MouseController mouse, CameraController cameraController, Editors.EntityEditor entityEditor)
        {
            this.Graphics = graphics;
            this.WorkspaceManager = workspace;
            this.Device = device;
            this.SpriteBatch = spriteBatch;
            this.GameTimer = gameTimer;
            this.Window = window;
            this.Content = content;
            this.FrameService = frameService;
            this.Gui = imGui;
            this.CubeMapGenerator = cubeMapGenerator;
            this.IrradianceMapGenerator = irradianceMapGenerator;
            this.EnvironmentMapGenerator = environmentMapGenerator;
            this.BrdfLutGenerator = brdfLutGenerator;
            this.Entities = entities;
            this.Components = components;
            this.Keyboard = keyboard;
            this.Mouse = mouse;
            this.CameraController = cameraController;
            this.EntityEditor = entityEditor;
            this.Content.Push("basics");

            this.SkyboxNames = new string[]
            {
                "Skyboxes/Circus/Circus_Backstage_3k",
                "Skyboxes/Industrial/fin4_Bg",
                "Skyboxes/Milkyway/Milkyway_small",
                "Skyboxes/Grid/testgrid",
                "Skyboxes/Loft/Newport_Loft_Ref"
            };

            this.SkyboxTextures = new TextureCube[this.SkyboxNames.Length];
            this.IrradianceTextures = new TextureCube[this.SkyboxNames.Length];
            this.EnvironmentTextures = new TextureCube[this.SkyboxNames.Length];
            for (var i = 0; i < this.SkyboxNames.Length; i++)
            {
                this.Content.Push("generator");
                var equiRect = this.Content.Load<Texture2D>(this.SkyboxNames[i]);
                this.SkyboxTextures[i] = this.CubeMapGenerator.Generate(equiRect);
                this.IrradianceTextures[i] = this.IrradianceMapGenerator.Generate(equiRect);
                this.EnvironmentTextures[i] = this.EnvironmentMapGenerator.Generate(equiRect);

                this.Content.Pop();

                this.Content.Link(this.SkyboxTextures[i]);
            }

            this.FrameService.BrdfLutTexture = this.BrdfLutGenerator.Generate();

            this.FrameService.Skybox = SkyboxGenerator.Generate(this.Device, this.SkyboxTextures[0], this.IrradianceTextures[0], this.EnvironmentTextures[0]);
            this.RenderPipeline = renderPipelineBuilder.Build();

            var red = new Texture2D(this.Device, 1, 1);
            red.SetData(new Color[] { Color.White });
            this.Content.Link(red);

            var normal = new Texture2D(this.Device, 1, 1);
            normal.SetData(new Color[] { new Color(0.5f, 0.5f, 1.0f) });
            this.Content.Link(normal);

            var blue = this.Content.Load<Texture2D>("Textures/Blue");
            var bumps = this.Content.Load<Texture2D>("Textures/Bricks_Normal");

            var rows = 7;
            var columns = 7;
            var spacing = 2.5f;
            var geometry = SphereGenerator.Generate(this.Device, 15);
            for (var row = 0; row < rows; row++)
            {
                var metalicness = row / (float)rows;
                for (var col = 0; col < columns; col++)
                {
                    var roughness = Math.Clamp(col / (float)columns, 0.05f, 1.0f);
                    var material = new Material(red, normal, metalicness, roughness);

                    var position = new Vector3((col - (columns / 2.0f)) * spacing, (row - (rows / 2.0f)) * spacing, 0.0f);
                    var transform = Matrix.CreateTranslation(position);
                    this.CreateSphere(geometry, material, transform);
                }
            }

            var backgroundGeometry = CubeGenerator.Generate(this.Device);
            this.CreateSphere(backgroundGeometry, new Material(blue, bumps, 1.0f, 0.1f), Matrix.CreateScale(20, 20, 1) * Matrix.CreateTranslation(Vector3.Forward * 20));

            var pointLightComponent = new PointLightComponent(this.Entities.Create(), new Vector3(-10, 10, 10), Color.Red, 300.0f);
            this.Components.Add(pointLightComponent);

            var pointLightComponent2 = new PointLightComponent(this.Entities.Create(), new Vector3(10, 10, 10), Color.Blue, 300.0f);
            this.Components.Add(pointLightComponent2);

            var pointLightComponent3 = new PointLightComponent(this.Entities.Create(), new Vector3(-10, -10, 10), Color.Green, 300.0f);
            this.Components.Add(pointLightComponent3);

            var pointLightComponent4 = new PointLightComponent(this.Entities.Create(), new Vector3(10, -10, 10), Color.White, 300.0f);
            this.Components.Add(pointLightComponent4);

            this.FrameCounter = new FrameCounter();
        }

        private void CreateSphere(Geometry geometry, Material material, Matrix transform)
        {
            var entity = this.Entities.Create();
            var component = new GeometryComponent(entity, geometry, material);
            this.Components.Add(component);

            var body = new TransformComponent(entity, transform);
            this.Components.Add(body);
        }

        internal bool Update(GameTime gameTime)
        {
            this.Keyboard.Update();
            this.Mouse.Update();

            if (this.Keyboard.Pressed(Keys.Escape))
            {
                return false;
            }

            if (this.Keyboard.Released(Keys.F12))
            {
                this.renderUi = !this.renderUi;
            }

            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.CameraController.Update(this.FrameService.Camera, elapsed);

            if (this.FrameCounter.Update(gameTime))
            {
                this.Window.Title = $"Editor :: {this.FrameCounter.MillisecondsPerFrame:F2}ms, {this.FrameCounter.FramesPerSecond} fps, {this.FrameService.GetBufferSize() * 0.000001f} MB";
            }

            return true;
        }

        internal void Draw(GameTime gameTime)
        {
            this.RunPipeline();

            this.Device.SetRenderTarget(null);

            this.RenderToViewport(this.FrameService.PBuffer.ToneMap);
            if (this.renderUi)
            {
                this.Gui.BeforeLayout(gameTime);
                this.ShowMainMenuBar();

                ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);
                this.WorkspaceManager.RenderWindows();

                this.EntityEditor.Draw();

                this.Gui.AfterLayout();
            }
        }

        private void RunPipeline()
        {
            this.RenderPipeline.Run();
            this.RenderPipeline.Wait();
        }

        private void ShowMainMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Windows"))
                {
                    ImGui.Checkbox("Entity Window", ref this.EntityEditor.ShowEntityWindow);
                    ImGui.Checkbox("Component Window", ref this.EntityEditor.ShowComponentWindow);

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("View"))
                {
                    var vsync = this.Graphics.SynchronizeWithVerticalRetrace;
                    ImGui.Checkbox("VSync", ref vsync);
                    this.Graphics.SynchronizeWithVerticalRetrace = vsync;
                    this.GameTimer.IsFixedTimeStep = vsync;
                    this.Graphics.ApplyChanges();

                    if (ImGui.ListBox("Skybox", ref this.currentSkyboxTexture, this.SkyboxNames, this.SkyboxTextures.Length))
                    {
                        this.FrameService.Skybox.Texture = this.SkyboxTextures[this.currentSkyboxTexture];
                        this.FrameService.Skybox.Irradiance = this.IrradianceTextures[this.currentSkyboxTexture];
                        this.FrameService.Skybox.Environment = this.EnvironmentTextures[this.currentSkyboxTexture];
                    }

                    ImGui.EndMenu();
                }

                this.WorkspaceManager.RenderMainMenuItems();

                ImGui.EndMainMenuBar();
            }
        }

        private void RenderToViewport(RenderTarget2D renderTarget)
        {
            this.SpriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.Opaque,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            this.SpriteBatch.Draw(
                renderTarget,
                new Rectangle(0, 0, this.Graphics.PreferredBackBufferWidth, this.Graphics.PreferredBackBufferHeight),
                null,
                Color.White);

            this.SpriteBatch.End();
        }

        public void Stop()
        {
            this.WorkspaceManager.Save();
            this.RenderPipeline.Stop();
        }
    }
}
