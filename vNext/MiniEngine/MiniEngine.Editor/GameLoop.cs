﻿using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using MiniEngine.Editor.Controllers;
using MiniEngine.Graphics;
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

        public GameLoop(Register registerDelegate, EntityAdministrator entityAdministator, ComponentAdministrator componentAdministrator, RenderPipelineBuilder renderPipelineBuilder,
            KeyboardController keyboard, MouseController mouse, CameraController cameraController)
        {
            this.RegisterDelegate = registerDelegate;
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

            this.frameService = new FrameService(this.Graphics.GraphicsDevice);
            this.RegisterDelegate(this.frameService);

            this.gui = new ImGuiRenderer(this.Graphics.GraphicsDevice, this.Window);

            this.renderPipeline = this.RenderPipelineBuilder.Build();
            this.spriteBatch = new SpriteBatch(this.Graphics.GraphicsDevice);

            var renderTargetSet = this.frameService.RenderTargetSet;
            renderTargetSet.Diffuse.Tag = this.gui.BindTexture(renderTargetSet.Diffuse);
            renderTargetSet.Normal.Tag = this.gui.BindTexture(renderTargetSet.Normal);
            renderTargetSet.Depth.Tag = this.gui.BindTexture(renderTargetSet.Depth);
            renderTargetSet.Combine.Tag = this.gui.BindTexture(renderTargetSet.Combine);

            var entity = this.EntityAdministator.Create();
            var blue = this.Content.Load<Texture2D>(@"Textures\Blue");
            var normal = this.Content.Load<Texture2D>(@"Textures\Bricks_Normal");
            var geometry = SpherifiedCubeGenerator.Generate(entity, 15, blue, normal);
            this.Components.Add(geometry);

            var body = new TransformComponent(entity, Matrix.CreateTranslation(Vector3.Forward * 3));
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
                this.RenderToWindow("Combine", this.frameService.RenderTargetSet.Combine);

                this.RenderToWindow("RenderTargets", this.frameService.RenderTargetSet.Diffuse);
                this.RenderToWindow("RenderTargets", this.frameService.RenderTargetSet.Depth);
                this.RenderToWindow("RenderTargets", this.frameService.RenderTargetSet.Normal);
            }
            else
            {
                this.RenderToViewport(this.frameService.RenderTargetSet.Combine);
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
                    ImGui.EndMenu();
                }
                ImGui.EndMainMenuBar();
            }
        }

        private void RenderToWindow(string title, RenderTarget2D renderTarget)
        {
            if (ImGui.Begin(title))
            {
                var width = ImGui.GetWindowWidth();
                var height = ImGui.GetWindowHeight() - (ImGui.GetFrameHeightWithSpacing() * 2);
                var imageSize = FitToBounds(renderTarget.Width, renderTarget.Height, width, height);

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
                DepthStencilState.Default,
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