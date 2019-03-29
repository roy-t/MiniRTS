using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering;
using MiniEngine.Systems;
using MiniEngine.Telemetry;
using MiniEngine.UI;
using MiniEngine.Units;
using System;

using System.Linq;

namespace MiniEngine
{
    public sealed class GameLoop : Game
    {        
        private readonly GraphicsDeviceManager Graphics;        
        private Injector injector;
                
        private PerspectiveCamera camera;
        private SpriteBatch spriteBatch;        
                
        private OutlineFactory outlineFactory;
        private DeferredRenderPipeline renderPipeline;
        private EntityCreator entityCreator;
        private EntityController entityController;
        private EntityLinker entityLinker;
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
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.camera = new PerspectiveCamera(this.GraphicsDevice.Viewport);

            this.injector = new Injector(this.GraphicsDevice, this.Content);
            this.entityCreator = this.injector.Resolve<EntityCreator>();
            this.entityController = this.injector.Resolve<EntityController>();
            this.entityLinker = this.injector.Resolve<EntityLinker>();
            this.outlineFactory = this.injector.Resolve<OutlineFactory>();
                                    
            this.renderPipeline = this.injector.Resolve<DeferredRenderPipeline>();

            this.sceneSelector = new SceneSelector(this.Content, this.injector);
            var renderTargetDescriber = new RenderTargetDescriber(this.renderPipeline.GetGBuffer());

            this.uiManager = new UIManager(this, this.spriteBatch, renderTargetDescriber, this.renderPipeline, this.camera, this.sceneSelector, this.injector);
          
            this.metricServer = this.injector.Resolve<IMetricServer>();
            this.metricServer.Start(7070);
        }       

        protected override void OnExiting(object sender, EventArgs args)
            => this.uiManager.Close(this.camera);

        protected override void Update(GameTime gameTime)
        {
            var elapsed = (Seconds)gameTime.ElapsedGameTime;
            this.sceneSelector.CurrentScene.Update(elapsed);
                       
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
            
            this.spriteBatch.End();

            this.uiManager.Render(this.GraphicsDevice.Viewport, gameTime);

            base.Draw(gameTime);
        }
    }
}

