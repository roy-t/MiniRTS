using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using MiniEngine.Editor.Controllers;
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
    internal sealed class GameLoop : IDisposable
    {
        private readonly GraphicsDeviceManager Graphics;
        private readonly GraphicsDevice Device;
        private readonly SpriteBatch SpriteBatch;
        private readonly GameTimer GameTimer;
        private readonly GameWindow Window;
        private readonly ContentStack Content;
        private readonly FrameService FrameService;
        private readonly ImGuiRenderer Gui;
        private readonly CubeMapGenerator CubeMapGenerator;
        private readonly EnvironmentMapGenerator EnvironmentMapGenerator;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly KeyboardController Keyboard;
        private readonly MouseController Mouse;
        private readonly CameraController CameraController;

        private readonly FrameCounter FrameCounter;

        private readonly string[] SkyboxNames;
        private readonly TextureCube[] SkyboxTextures;
        private int currentSkyboxTexture = 0;


        private readonly ParallelPipeline RenderPipeline;

        private bool docked = true;
        private bool showDemoWindow = false;

        public GameLoop(GraphicsDeviceManager graphics, GraphicsDevice device, SpriteBatch spriteBatch, GameTimer gameTimer, GameWindow window, ContentStack content, FrameService frameService, ImGuiRenderer imGui, CubeMapGenerator cubeMapGenerator, EnvironmentMapGenerator environmentMapGenerator, EntityAdministrator entities, ComponentAdministrator components, RenderPipelineBuilder renderPipelineBuilder, KeyboardController keyboard, MouseController mouse, CameraController cameraController)
        {
            this.Graphics = graphics;
            this.Device = device;
            this.SpriteBatch = spriteBatch;
            this.GameTimer = gameTimer;
            this.Window = window;
            this.Content = content;
            this.FrameService = frameService;
            this.Gui = imGui;
            this.CubeMapGenerator = cubeMapGenerator;
            this.EnvironmentMapGenerator = environmentMapGenerator;
            this.Entities = entities;
            this.Components = components;
            this.Keyboard = keyboard;
            this.Mouse = mouse;
            this.CameraController = cameraController;

            this.Content.Push("basics");

            this.SkyboxNames = new string[]
            {
                "Skyboxes/Industrial/fin4_Bg",
                "Skyboxes/Milkyway/Milkyway_small",
                "Skyboxes/Grid/testgrid",
                "Skyboxes/Loft/Newport_Loft_Ref"
            };

            this.SkyboxTextures = new TextureCube[this.SkyboxNames.Length];
            for (var i = 0; i < this.SkyboxNames.Length; i++)
            {
                this.Content.Push("generator");
                var equiRect = this.Content.Load<Texture2D>(this.SkyboxNames[i]);
                this.SkyboxTextures[i] = this.EnvironmentMapGenerator.Generate(equiRect); //environmentmap
                this.Content.Pop();

                this.Content.Link(this.SkyboxTextures[i]);
            }

            this.FrameService.Skybox = SkyboxGenerator.Generate(this.Device, this.SkyboxTextures[0]);
            this.FrameService.Camera.Move(Vector3.Backward * 10, Vector3.Forward);

            this.RenderPipeline = renderPipelineBuilder.Build();

            var gBuffer = this.FrameService.GBuffer;
            gBuffer.Diffuse.Tag = this.Gui.BindTexture(gBuffer.Diffuse);
            gBuffer.Material.Tag = this.Gui.BindTexture(gBuffer.Material);
            gBuffer.Normal.Tag = this.Gui.BindTexture(gBuffer.Normal);
            gBuffer.Depth.Tag = this.Gui.BindTexture(gBuffer.Depth);

            var lBuffer = this.FrameService.LBuffer;
            lBuffer.Light.Tag = this.Gui.BindTexture(lBuffer.Light);

            var pBuffer = this.FrameService.PBuffer;
            pBuffer.Combine.Tag = this.Gui.BindTexture(pBuffer.Combine);
            pBuffer.PostProcess.Tag = this.Gui.BindTexture(pBuffer.PostProcess);

            var red = new Texture2D(this.Device, 1, 1);
            red.SetData(new Color[] { Color.Red });
            this.Content.Link(red);

            var normal = new Texture2D(this.Device, 1, 1);
            normal.SetData(new Color[] { new Color(0.5f, 0.5f, 1.0f) });
            this.Content.Link(normal);

            var rows = 7;
            var columns = 7;
            var spacing = 2.5f;
            var geometry = SpherifiedCubeGenerator.Generate(this.Device, 15);
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

            var ambientLight = new AmbientLightComponent(this.Entities.Create(), Color.White);
            this.Components.Add(ambientLight);

            var pointLightComponent = new PointLightComponent(this.Entities.Create(), new Vector3(-10, 10, 10), Color.White, 300.0f);
            this.Components.Add(pointLightComponent);

            var pointLightComponent2 = new PointLightComponent(this.Entities.Create(), new Vector3(10, 10, 10), Color.White, 300.0f);
            this.Components.Add(pointLightComponent2);

            var pointLightComponent3 = new PointLightComponent(this.Entities.Create(), new Vector3(-10, -10, 10), Color.White, 300.0f);
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

            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.CameraController.Update(this.FrameService.Camera, elapsed);

            if (this.FrameCounter.Update(gameTime))
            {
                this.Window.Title = $"Editor :: {this.FrameCounter.MillisecondsPerFrame:F2}ms, {this.FrameCounter.FramesPerSecond} fps";
            }

            return true;
        }

        internal void Draw(GameTime gameTime)
        {
            this.RunPipeline();

            this.Device.SetRenderTarget(null);

            this.Gui.BeforeLayout(gameTime);
            this.ShowMainMenuBar();

            if (this.docked)
            {
                ImGui.DockSpaceOverViewport();
                this.RenderToWindow("PostProcess", "Combine", this.FrameService.PBuffer.Combine);

                this.RenderToWindow("RenderTargets", "Diffuse", this.FrameService.GBuffer.Diffuse);
                this.RenderToWindow("RenderTargets", "Material", this.FrameService.GBuffer.Material);
                this.RenderToWindow("RenderTargets", "Depth", this.FrameService.GBuffer.Depth);
                this.RenderToWindow("RenderTargets", "Normal", this.FrameService.GBuffer.Normal);

                this.RenderToWindow("RenderTargets", "Light", this.FrameService.LBuffer.Light); // TODO: light is invisible because a = 0!

                this.RenderToWindow("RenderTargets", "Combine", this.FrameService.PBuffer.Combine);
                this.RenderToWindow("RenderTargets", "PostProcess", this.FrameService.PBuffer.PostProcess);
            }
            else
            {
                this.RenderToViewport(this.FrameService.PBuffer.PostProcess);
            }

            if (this.showDemoWindow)
            {
                ImGui.ShowDemoWindow();
            }
            this.Gui.AfterLayout();
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
                if (ImGui.BeginMenu("View"))
                {
                    ImGui.Checkbox("Docked", ref this.docked);

                    var vsync = this.Graphics.SynchronizeWithVerticalRetrace;
                    ImGui.Checkbox("VSync", ref vsync);
                    this.Graphics.SynchronizeWithVerticalRetrace = vsync;
                    this.GameTimer.IsFixedTimeStep = vsync;
                    this.Graphics.ApplyChanges();

                    ImGui.Checkbox("Show Demo Window", ref this.showDemoWindow);

                    if (ImGui.ListBox("Skybox", ref this.currentSkyboxTexture, this.SkyboxNames, this.SkyboxTextures.Length))
                    {
                        this.FrameService.Skybox.Texture = this.SkyboxTextures[this.currentSkyboxTexture];
                    }

                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

        private void RenderToWindow(string window, string label, RenderTarget2D renderTarget)
        {
            if (ImGui.Begin(window))
            {
                var width = ImGui.GetWindowWidth();
                var height = ImGui.GetWindowHeight() - (ImGui.GetFrameHeightWithSpacing() * 2);
                var imageSize = FitToBounds(renderTarget.Width, renderTarget.Height, width, height);

                ImGui.Text(label);
                ImGui.Image((IntPtr)renderTarget.Tag, imageSize);

                ImGui.End();
            }
        }

        private static Vector2 FitToBounds(float sourceWidth, float sourceHeight, float boundsWidth, float boundsHeight)
        {
            var widthRatio = boundsWidth / sourceWidth;
            var heightRatio = boundsHeight / sourceHeight;

            var ratio = Math.Min(widthRatio, heightRatio);
            return new Vector2(sourceWidth * ratio, sourceHeight * ratio);
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

        public void Dispose()
        {
            this.RenderPipeline.Stop();
            this.Gui.Dispose();
            this.Content.Dispose();
        }
    }
}
