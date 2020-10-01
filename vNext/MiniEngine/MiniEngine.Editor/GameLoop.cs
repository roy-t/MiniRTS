using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry.Generators;
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
        private readonly EntityAdministrator EntityAdministator;
        private new readonly ComponentAdministrator Components;
        private readonly RenderPipelineBuilder RenderPipelineBuilder;

        private readonly FrameService FrameService;
        private readonly PerspectiveCamera PrimaryCamera;

        private readonly FrameCounter FrameCounter;

        private ParallelPipeline? renderPipeline;
        private SpriteBatch? spriteBatch;
        private RenderTarget2D? renderTarget;
        private IntPtr renderTargetBinding;

        private ImGuiRenderer? gui;
        private bool docked = true;
        private bool showDemoWindow = false;

        /*
         * Next steps:
         * - Give the geometry service a real shader
         * - Generate a sphere, start experimenting with SRGB and PBR
         */

        public GameLoop(Register registerDelegate, EntityAdministrator entityAdministator, ComponentAdministrator componentAdministrator, RenderPipelineBuilder renderPipelineBuilder, FrameService frameService)
        {
            this.RegisterDelegate = registerDelegate;
            this.EntityAdministator = entityAdministator;
            this.Components = componentAdministrator;
            this.RenderPipelineBuilder = renderPipelineBuilder;
            this.FrameService = frameService;

            this.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080,
                PreferMultiSampling = false,
                SynchronizeWithVerticalRetrace = true,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;

            this.PrimaryCamera = new PerspectiveCamera(this.Graphics.PreferredBackBufferWidth / (float)this.Graphics.PreferredBackBufferHeight);
            this.FrameCounter = new FrameCounter();
        }

        protected override void LoadContent()
        {
            this.RegisterDelegate(this.Graphics.GraphicsDevice);
            this.RegisterDelegate(this.Content);

            this.gui = new ImGuiRenderer(this.Graphics.GraphicsDevice, this.Window);

            this.renderPipeline = this.RenderPipelineBuilder.Build();
            this.spriteBatch = new SpriteBatch(this.Graphics.GraphicsDevice);

            this.renderTarget = new RenderTarget2D(this.Graphics.GraphicsDevice, this.Graphics.PreferredBackBufferWidth, this.Graphics.PreferredBackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);
            this.renderTargetBinding = this.gui.BindTexture(this.renderTarget);

            var entity = this.EntityAdministator.Create();
            var geometry = SpherifiedCubeGenerator.Generate(entity, 6);
            this.Components.Add(geometry);

            var body = new TransformComponent(entity);
            body.Matrix = Matrix.CreateTranslation(Vector3.Forward * 3);
            this.Components.Add(body);
        }

        protected override void UnloadContent()
        {
            this.renderPipeline!.Stop();
            this.gui?.Dispose();
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.FrameCounter.Update(gameTime))
            {
                this.Window.Title = $"Editor :: {this.FrameCounter.MillisecondsPerFrame:F2}ms, {this.FrameCounter.FramesPerSecond} fps";
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.FrameService.Camera = this.PrimaryCamera;
            this.RunPipeline();

            this.gui!.BeforeLayout(gameTime);
            this.ShowMainMenuBar();

            if (this.docked)
            {
                ImGui.DockSpaceOverViewport();
                this.RenderToWindow(this.renderTarget, this.renderTargetBinding);
            }
            else
            {
                this.RenderToViewport(this.renderTarget);
            }

            if (this.showDemoWindow)
            {
                ImGui.ShowDemoWindow();
            }
            this.gui!.AfterLayout();

            base.Draw(gameTime);
        }

        private void RunPipeline()
        {
            this.GraphicsDevice.SetRenderTarget(this.renderTarget);

            this.renderPipeline!.Run();
            this.renderPipeline!.Wait();

            this.GraphicsDevice.SetRenderTarget(null);
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
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

        private void RenderToWindow(RenderTarget2D? renderTarget, IntPtr renderTargetBinding)
        {
            if (ImGui.Begin("Output"))
            {
                var width = ImGui.GetWindowWidth();
                var height = ImGui.GetWindowHeight() - (ImGui.GetFrameHeightWithSpacing() * 2);
                var imageSize = FitToBounds(renderTarget?.Width ?? 1, renderTarget?.Height ?? 1, width, height);
                ImGui.Image(renderTargetBinding, imageSize);

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

        private void RenderToViewport(RenderTarget2D? renderTarget)
        {
            this.spriteBatch!.Begin(
                SpriteSortMode.Immediate,
                BlendState.Opaque,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            this.spriteBatch!.Draw(
                renderTarget,
                new Rectangle(0, 0, this.Graphics.PreferredBackBufferWidth, this.Graphics.PreferredBackBufferHeight),
                null,
                Color.White);

            this.spriteBatch!.End();
        }
    }
}
