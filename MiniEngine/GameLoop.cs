using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Input;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Scenes;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Utilities;

namespace MiniEngine
{
    public class GameLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;        

        private Injector injector;

        private KeyboardInput keyboardInput;
        private MouseInput mouseInput;

        private bool detailView = true;
        private int viewIndex;
        private int viewOptions = 4;

        private PerspectiveCamera perspectiveCamera;
        private SpriteBatch spriteBatch;
        private IReadOnlyList<IScene> scenes;
        private int currentSceneIndex;
        private DebugController debugController;
        private DeferredRenderer renderer;
        private EntityController entityController;


        public GameLoop()
        {
            this.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 2560,
                PreferredBackBufferHeight = 1440,
                SynchronizeWithVerticalRetrace = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };          

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }       

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.perspectiveCamera = new PerspectiveCamera(this.GraphicsDevice.Viewport);

            this.injector = new Injector(this.GraphicsDevice, this.Content);

            this.keyboardInput = this.injector.Resolve<KeyboardInput>();
            this.mouseInput = this.injector.Resolve<MouseInput>();                        
                        
            this.entityController = this.injector.Resolve<EntityController>();
            this.debugController = this.injector.Resolve<DebugControllerFactory>().Build(this.perspectiveCamera);            

            this.renderer = this.injector.Resolve<DeferredRenderer>();

            this.scenes = this.injector.ResolveAll<IScene>()
                              .ToList()
                              .AsReadOnly();


            foreach (var scene in this.scenes)
            {
                scene.LoadContent(this.Content);
            }

            this.scenes[0].Set();            
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            this.debugController.Update(gameTime.ElapsedGameTime);

            this.scenes[this.currentSceneIndex].Update(gameTime.ElapsedGameTime);

            this.keyboardInput.Update();
            this.mouseInput.Update();

            // Do not handle input if game window is not activated
            if (!this.IsActive)
                return;
            
            var inputHandled = this.debugController.Update(gameTime.ElapsedGameTime);
            if (inputHandled)
                return;


            if (this.keyboardInput.Click(Keys.Escape))
            {
                Exit();
            }            

            if (this.keyboardInput.Click(Keys.OemTilde))
            {
                this.detailView = !this.detailView;
            }

            if (this.keyboardInput.Click(Keys.Tab))
            {
                this.currentSceneIndex = (this.currentSceneIndex + 1) % this.scenes.Count;
                this.entityController.DestroyAllEntities();

                this.scenes[this.currentSceneIndex].Set();

            }

            if (this.keyboardInput.Click(Keys.F))
            {
                this.IsFixedTimeStep = !this.IsFixedTimeStep;
            }

            if (this.keyboardInput.Click(Keys.OemPlus))
            {
                this.viewIndex = (this.viewIndex + 1) % this.viewOptions;
            }
            else if (this.keyboardInput.Click(Keys.OemMinus))
            {
                this.viewIndex = (this.viewIndex + this.viewOptions - 1) % this.viewOptions;
            }

            if (this.keyboardInput.Click(Keys.X))
            {
                this.renderer.EnableFXAA = !this.renderer.EnableFXAA;
            }

            if (this.keyboardInput.Click(Keys.Scroll))
            {
                this.entityController.DescribeAllEntities();
            }                       

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.Window.Title = $"{gameTime.ElapsedGameTime.TotalMilliseconds:F2}ms, {(1.0f / gameTime.ElapsedGameTime.TotalSeconds):F2} fps, Fixed Time Step: {this.IsFixedTimeStep} (press 'F' so switch). Input State: {this.debugController.DescribeState()}";

            this.GraphicsDevice.Clear(Color.CornflowerBlue);
            var result = this.renderer.Render(this.perspectiveCamera);
            this.GraphicsDevice.SetRenderTarget(null);

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


            var gBuffer = this.renderer.GetIntermediateRenderTargets();
            this.viewOptions = gBuffer.Length;

            if (this.detailView)
            {
                var step = this.GraphicsDevice.Viewport.Width / gBuffer.Length;
                for (var i = 0; i < gBuffer.Length; i++)
                {
                    var target = gBuffer[i];
                    this.spriteBatch.Draw(
                        target,
                        new Vector2(step * i, 0),
                        null,
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        1.0f / gBuffer.Length,
                        SpriteEffects.None,
                        0);
                }
            }
            else
            {
                this.spriteBatch.Draw(
                    gBuffer[this.viewIndex],
                    new Vector2(0, 0),
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0);
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
