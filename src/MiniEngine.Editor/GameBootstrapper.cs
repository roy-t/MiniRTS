using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Editor
{
    internal sealed class GameBootstrapper : Game
    {
        private readonly GraphicsDeviceManager Graphics;

        private readonly Register RegisterDelegate;
        private readonly RegisterAs RegisterAsDelegate;
        private readonly Func<GameLoop> GameLoopFactory;

        private GameLoop gameLoop = null!;

        public GameBootstrapper(Register registerDelegate, RegisterAs registerAsDelegate, Func<GameLoop> gameLoopFactory)
        {
            this.RegisterDelegate = registerDelegate;
            this.RegisterAsDelegate = registerAsDelegate;
            this.GameLoopFactory = gameLoopFactory;
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
        }

        protected override void LoadContent()
        {
            this.RegisterDelegate(this.Graphics);
            this.RegisterDelegate(this.Graphics.GraphicsDevice);
            this.RegisterDelegate(this.Content);
            this.RegisterAsDelegate(this.Window, typeof(GameWindow));

            var gameTimer = new GameTimer(this);
            this.RegisterDelegate(gameTimer);

            this.gameLoop = this.GameLoopFactory();
        }

        protected override void UnloadContent()
        {
            this.gameLoop.Stop();
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!this.gameLoop.Update(gameTime))
            {
                this.Exit();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.gameLoop.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
