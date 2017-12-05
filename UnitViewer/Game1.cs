using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RenderEngine;

namespace UnitViewer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager Graphics;

        private SpriteBatch spriteBatch;
        private Texture2D texture;
        private Camera camera;
        private RenderSystem renderSystem;

        public Game1()
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.texture = this.Content.Load<Texture2D>("Texture");

            this.camera = new Camera(this.GraphicsDevice.Viewport);

            var clearEffect = this.Content.Load<Effect>("ClearEffect");
            this.renderSystem = new RenderSystem(this.GraphicsDevice, clearEffect, this.camera);


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
