﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.GameLogic;
using MiniEngine.Net;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering;
using MiniEngine.Telemetry;
using MiniEngine.UI;
using MiniEngine.Units;

namespace MiniEngine
{
    public sealed class GameLoop : Game
    {
        [SuppressMessage("Code Quality", "IDE0052:Remove unread private members", Justification = "Reference required to prevent collection")]
        private readonly GraphicsDeviceManager Graphics;

        private Injector injector;

        private PerspectiveCamera camera;
        private SpriteBatch spriteBatch;

        private DeferredRenderPipeline renderPipeline;
        private UI.Input.KeyboardInput keyboard;

        private IMetricServer metricServer;

        private UIManager uiManager;
        private SceneSelector sceneSelector;

        private Server server;
        private Client client;

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
        }

        protected override void LoadContent()
        {
            this.camera = new PerspectiveCamera(this.GraphicsDevice.Viewport);
            this.injector = new Injector(this);

            this.injector.Resolve<Content>().LoadContent(this.Content);

            this.spriteBatch = this.injector.Resolve<SpriteBatch>();
            this.renderPipeline = this.injector.Resolve<DeferredRenderPipeline>();
            this.sceneSelector = this.injector.Resolve<SceneSelector>();

            this.server = this.injector.Resolve<Server>();
            this.client = this.injector.Resolve<Client>();
            this.keyboard = this.injector.Resolve<UI.Input.KeyboardInput>();

            this.metricServer = this.injector.Resolve<IMetricServer>();
            this.metricServer.Start(7070);


            this.uiManager = this.injector.Resolve<UIManager>();
            this.uiManager.LoadState();


            if (this.uiManager.State.NetState.AutoStartServer)
            {
                this.server.Start(Server.DefaultServerPort);
            }

            if (this.uiManager.State.NetState.AutoStartClient)
            {
                this.client.Connect(new IPEndPoint(IPAddress.Loopback, Server.DefaultServerPort));
            }
        }

        protected override void UnloadContent()
        {
            this.renderPipeline?.Dispose();
            base.UnloadContent();
        }

        protected override void OnExiting(object sender, EventArgs args)
            => this.uiManager.Close(this.camera);

        protected override void Update(GameTime gameTime)
        {
            var elapsed = (Seconds)gameTime.ElapsedGameTime;
            this.sceneSelector.CurrentScene.Update(this.camera, elapsed);

            if (this.server.IsRunning)
            {
                this.server.Update();
            }

            if (this.client.IsConnected)
            {
                this.client.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.Window.Title = $"{gameTime.ElapsedGameTime.TotalMilliseconds:F2}ms, {1.0f / gameTime.ElapsedGameTime.TotalSeconds:F2} fps.";
            this.Window.Title +=
                $" Camera ({this.camera.Position.X:F2}, {this.camera.Position.Y:F2}, {this.camera.Position.Z:F2})";


            var skybox = this.sceneSelector.CurrentScene.Skybox;
            var result = this.renderPipeline.Render(this.camera, (float)gameTime.ElapsedGameTime.TotalSeconds, skybox);

            if (this.keyboard.Click(Keys.PrintScreen))
            {
                var path = System.IO.Path.GetFullPath($"Screenshot {this.sceneSelector.CurrentScene.Name} - {DateTime.Now.Ticks}");
                result.SaveAsPng(File.Create(path), result.Width, result.Height);
                Debug.WriteLine($"Screenshot saved in {path}");
            }

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

            this.spriteBatch.End();

            this.uiManager.Render(this.sceneSelector.CurrentScene, this.camera, this.GraphicsDevice.Viewport, gameTime);

            base.Draw(gameTime);
        }
    }
}

