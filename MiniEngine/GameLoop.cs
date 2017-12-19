using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Rendering;

namespace MiniEngine
{    
    public class GameLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;

        private SpriteBatch spriteBatch;
        private Texture2D texture;
        private Scene scene;
        private RenderSystem renderSystem;
        

        public GameLoop()
        {
            this.Graphics = new GraphicsDeviceManager(this);
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
            this.scene = new Scene(this.GraphicsDevice, camera);
            this.scene.LoadContent(this.Content);

            var clearEffect = this.Content.Load<Effect>("ClearEffect");
            this.renderSystem = new RenderSystem(this.GraphicsDevice, clearEffect, this.scene);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.renderSystem.Render();

            this.spriteBatch.Begin();

            var gBuffer = this.renderSystem.GetGBuffer();
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
                    0.33f,
                    SpriteEffects.None,
                    0);
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
