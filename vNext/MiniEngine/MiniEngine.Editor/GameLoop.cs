using System;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using MiniEngine.Editor.Controllers;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Effects;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.Skybox;
using MiniEngine.Gui;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;
using MiniEngine.Systems.Pipeline;

namespace MiniEngine.Editor
{
    public class GameLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private readonly Register RegisterDelegate;
        private readonly Resolve ResolveDelegate;
        private readonly EntityAdministrator EntityAdministator;
        private new readonly ComponentAdministrator Components;
        private readonly RenderPipelineBuilder RenderPipelineBuilder;

        private readonly KeyboardController Keyboard;
        private readonly MouseController Mouse;
        private readonly CameraController CameraController;

        private readonly FrameCounter FrameCounter;

        private FrameService frameService = null!;
        private ParallelPipeline renderPipeline = null!;
        private SpriteBatch spriteBatch = null!;
        private ImGuiRenderer gui = null!;

        private bool docked = true;
        private bool showDemoWindow = false;

        private int currentSkyboxTexture = 0;
        private string[] skyboxNames = null!;
        private Texture2D[] skyboxTextures = null!;

        public GameLoop(Register registerDelegate, Resolve resolveDelegate, EntityAdministrator entityAdministator, ComponentAdministrator componentAdministrator, RenderPipelineBuilder renderPipelineBuilder,
            KeyboardController keyboard, MouseController mouse, CameraController cameraController)
        {
            this.RegisterDelegate = registerDelegate;
            this.ResolveDelegate = resolveDelegate;
            this.EntityAdministator = entityAdministator;
            this.Components = componentAdministrator;
            this.RenderPipelineBuilder = renderPipelineBuilder;
            this.Keyboard = keyboard;
            this.Mouse = mouse;
            this.CameraController = cameraController;
            this.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                PreferredBackBufferFormat = SurfaceFormat.ColorSRgb,
                PreferMultiSampling = false,
                SynchronizeWithVerticalRetrace = true,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;

            this.FrameCounter = new FrameCounter();
        }

        protected override void LoadContent()
        {
            this.RegisterDelegate(this.Graphics.GraphicsDevice);
            this.RegisterDelegate(this.Content);

            this.skyboxTextures = new Texture2D[]
            {
                this.Content.Load<Texture2D>("Skyboxes/Industrial/fin4_Bg"),
                this.Content.Load<Texture2D>("Skyboxes/Milkyway/Milkyway_small"),
                this.Content.Load<Texture2D>("Skyboxes/Grid/testgrid"),
                this.Content.Load<Texture2D>("Skyboxes/Loft/Newport_Loft_Ref")
            };

            this.skyboxNames = this.skyboxTextures.Select(s => s.Name).ToArray();

            var skybox = SkyboxGenerator.Generate(this.skyboxTextures[0]);

            this.frameService = new FrameService(this.Graphics.GraphicsDevice, skybox);
            this.RegisterDelegate(this.frameService);

            this.frameService.Camera.Move(Vector3.Backward * 10, Vector3.Forward);

            var effectFactory = (EffectFactory)this.ResolveDelegate(typeof(EffectFactory));
            this.gui = new ImGuiRenderer(this.Graphics.GraphicsDevice, this.Window, effectFactory);

            this.renderPipeline = this.RenderPipelineBuilder.Build();
            this.spriteBatch = new SpriteBatch(this.Graphics.GraphicsDevice);

            var gBuffer = this.frameService.GBuffer;
            gBuffer.Diffuse.Tag = this.gui.BindTexture(gBuffer.Diffuse);
            gBuffer.Material.Tag = this.gui.BindTexture(gBuffer.Material);
            gBuffer.Normal.Tag = this.gui.BindTexture(gBuffer.Normal);
            gBuffer.Depth.Tag = this.gui.BindTexture(gBuffer.Depth);

            var lBuffer = this.frameService.LBuffer;
            lBuffer.Light.Tag = this.gui.BindTexture(lBuffer.Light);

            var pBuffer = this.frameService.PBuffer;
            pBuffer.Combine.Tag = this.gui.BindTexture(pBuffer.Combine);
            pBuffer.PostProcess.Tag = this.gui.BindTexture(pBuffer.PostProcess);


            var red = new Texture2D(this.GraphicsDevice, 1, 1);
            red.SetData(new Color[] { Color.Red });

            var normal = new Texture2D(this.GraphicsDevice, 1, 1);
            normal.SetData(new Color[] { new Color(0.5f, 0.5f, 1.0f) });

            var rows = 7;
            var columns = 7;
            var spacing = 2.5f;
            for (var row = 0; row < rows; row++)
            {
                var metalicness = row / (float)rows;
                for (var col = 0; col < columns; col++)
                {
                    var roughness = Math.Clamp(col / (float)columns, 0.05f, 1.0f);
                    var material = new Material(red, normal, metalicness, roughness);

                    var position = new Vector3((col - (columns / 2.0f)) * spacing, (row - (rows / 2.0f)) * spacing, 0.0f);
                    var transform = Matrix.CreateTranslation(position);
                    this.CreateSphere(material, transform);
                }
            }

            var ambientLight = new AmbientLightComponent(this.EntityAdministator.Create(), Color.White);
            this.Components.Add(ambientLight);

            var pointLightComponent = new PointLightComponent(this.EntityAdministator.Create(), new Vector3(-10, 10, 10), Color.White, 300.0f);
            this.Components.Add(pointLightComponent);

            var pointLightComponent2 = new PointLightComponent(this.EntityAdministator.Create(), new Vector3(10, 10, 10), Color.White, 300.0f);
            this.Components.Add(pointLightComponent2);

            var pointLightComponent3 = new PointLightComponent(this.EntityAdministator.Create(), new Vector3(-10, -10, 10), Color.White, 300.0f);
            this.Components.Add(pointLightComponent3);

            var pointLightComponent4 = new PointLightComponent(this.EntityAdministator.Create(), new Vector3(10, -10, 10), Color.White, 300.0f);
            this.Components.Add(pointLightComponent4);
        }

        private void CreateSphere(Material material, Matrix transform)
        {
            var entity = this.EntityAdministator.Create();
            var geometry = SpherifiedCubeGenerator.Generate(entity, 15, material);
            this.Components.Add(geometry);

            var body = new TransformComponent(entity, transform);
            this.Components.Add(body);
        }

        protected override void UnloadContent()
        {
            this.renderPipeline.Stop();
            this.gui.Dispose();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            this.Keyboard.Update();
            this.Mouse.Update();

            if (this.Keyboard.Pressed(Keys.Escape))
            {
                this.Exit();
            }

            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.CameraController.Update(this.frameService.Camera, elapsed);

            if (this.FrameCounter.Update(gameTime))
            {
                this.Window.Title = $"Editor :: {this.FrameCounter.MillisecondsPerFrame:F2}ms, {this.FrameCounter.FramesPerSecond} fps";
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.RunPipeline();

            this.GraphicsDevice.SetRenderTarget(null);

            this.gui.BeforeLayout(gameTime);
            this.ShowMainMenuBar();

            if (this.docked)
            {
                ImGui.DockSpaceOverViewport();
                this.RenderToWindow("PostProcess", "Combine", this.frameService.PBuffer.Combine);

                this.RenderToWindow("RenderTargets", "Diffuse", this.frameService.GBuffer.Diffuse);
                this.RenderToWindow("RenderTargets", "Material", this.frameService.GBuffer.Material);
                this.RenderToWindow("RenderTargets", "Depth", this.frameService.GBuffer.Depth);
                this.RenderToWindow("RenderTargets", "Normal", this.frameService.GBuffer.Normal);

                this.RenderToWindow("RenderTargets", "Light", this.frameService.LBuffer.Light); // TODO: light is invisible because a = 0!

                this.RenderToWindow("RenderTargets", "Combine", this.frameService.PBuffer.Combine);
                this.RenderToWindow("RenderTargets", "PostProcess", this.frameService.PBuffer.PostProcess);
            }
            else
            {
                this.RenderToViewport(this.frameService.PBuffer.PostProcess);
            }

            if (this.showDemoWindow)
            {
                ImGui.ShowDemoWindow();
            }
            this.gui.AfterLayout();

            base.Draw(gameTime);
        }

        private void RunPipeline()
        {
            this.renderPipeline.Run();
            this.renderPipeline.Wait();
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
                    this.IsFixedTimeStep = vsync;
                    this.Graphics.ApplyChanges();

                    ImGui.Checkbox("Show Demo Window", ref this.showDemoWindow);

                    if (ImGui.ListBox("Skybox", ref this.currentSkyboxTexture, this.skyboxNames, this.skyboxTextures.Length))
                    {
                        this.frameService.Skybox.Texture = this.skyboxTextures[this.currentSkyboxTexture];
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
            this.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.Opaque,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            this.spriteBatch.Draw(
                renderTarget,
                new Rectangle(0, 0, this.Graphics.PreferredBackBufferWidth, this.Graphics.PreferredBackBufferHeight),
                null,
                Color.White);

            this.spriteBatch.End();
        }
    }
}
