using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.GameLogic;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering;
using MiniEngine.Scenes;
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

        private IMetricServer metricServer;

        private UIManager uiManager;
        private SceneSelector sceneSelector;

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

            this.spriteBatch = this.injector.Resolve<SpriteBatch>();
            this.renderPipeline = this.injector.Resolve<DeferredRenderPipeline>();
            this.sceneSelector = this.injector.Resolve<SceneSelector>();

            this.metricServer = this.injector.Resolve<IMetricServer>();
            this.metricServer.Start(7070);

            this.injector.Resolve<Content>().LoadContent(this.Content);

            this.uiManager = this.injector.Resolve<UIManager>();
            this.uiManager.LoadState();
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.Window.Title = $"{gameTime.ElapsedGameTime.TotalMilliseconds:F2}ms, {1.0f / gameTime.ElapsedGameTime.TotalSeconds:F2} fps.";
            this.Window.Title +=
                $" Camera ({this.camera.Position.X:F2}, {this.camera.Position.Y:F2}, {this.camera.Position.Z:F2})";


            var skybox = this.sceneSelector.CurrentScene.Skybox;
            var result = this.renderPipeline.Render(this.camera, (float)gameTime.ElapsedGameTime.TotalSeconds, skybox);

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

