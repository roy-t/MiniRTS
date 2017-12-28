using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Rendering;

namespace MiniEngine
{    
    public class GameLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private readonly KeyboardInput KeyboardInput;

        private bool detailView = true;
        private int viewIndex = 0;
        private int viewOptions = 4;

        private SpriteBatch spriteBatch;
        private Texture2D texture;
        private Scene scene;
        private CameraController cameraController;
        private RenderSystem renderSystem;
        

        public GameLoop()
        {
            this.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1080,
                PreferredBackBufferHeight = 768,
                SynchronizeWithVerticalRetrace = false                         
            };            

            this.KeyboardInput = new KeyboardInput();

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.texture = this.Content.Load<Texture2D>("Texture");
            
            var camera = new Camera(this.GraphicsDevice.Viewport);
            this.cameraController = new CameraController(this.KeyboardInput, camera);
            this.scene = new Scene(this.GraphicsDevice, camera);
            this.scene.LoadContent(this.Content);

            var clearEffect = this.Content.Load<Effect>("Clear");
            var combineEffect = this.Content.Load<Effect>("Combine");
            var directionalLightEffect = this.Content.Load<Effect>("DirectionalLight");
            var pointLightEffect = this.Content.Load<Effect>("PointLight");
            var sphere = this.Content.Load<Model>("Sphere");
            this.renderSystem = new RenderSystem(this.GraphicsDevice, clearEffect, directionalLightEffect, pointLightEffect, sphere, combineEffect, this.scene);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            this.KeyboardInput.Update();
            if (this.KeyboardInput.Click(Keys.OemTilde))
            {
                this.detailView = !this.detailView;
            }

            if (this.KeyboardInput.Click(Keys.F))
            {
                this.IsFixedTimeStep = !this.IsFixedTimeStep;
            }

            if (this.KeyboardInput.Click(Keys.OemPlus))
            {
                this.viewIndex = (this.viewIndex + 1) % this.viewOptions;
            }
            else if (this.KeyboardInput.Click(Keys.OemMinus))
            {
                this.viewIndex = (this.viewIndex + this.viewOptions - 1) % this.viewOptions;
            }

            this.cameraController.Update(gameTime.ElapsedGameTime);
            this.scene.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.renderSystem.Render();
            this.Window.Title = $"{gameTime.ElapsedGameTime.TotalMilliseconds:F2}ms, {(1.0f / gameTime.ElapsedGameTime.TotalSeconds):F2} fps, Fixed Time Step: {this.IsFixedTimeStep} (press 'F' so switch)";

            this.spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            var gBuffer = this.renderSystem.GetIntermediateRenderTargets();
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
