using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using System;

namespace MiniEngine.Editor
{
    internal sealed class GameBootstrapper : Game
    {
        private readonly GraphicsDeviceManager Graphics;

        private readonly Register RegisterDelegate;
        private readonly RegisterAs RegisterAsDelegate;
        private readonly Resolve Resolve;

        private IGameLoop gameLoop = null!;

        public GameBootstrapper(Register registerDelegate, RegisterAs registerAsDelegate, Resolve resolve)
        {
            this.RegisterDelegate = registerDelegate;
            this.RegisterAsDelegate = registerAsDelegate;

            var loaded = RenderDoc.Load(out var renderDoc);
            if (loaded && renderDoc != null)
            {
                registerAsDelegate(renderDoc, typeof(RenderDoc));
            }

            this.Resolve = resolve;
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
            this.RegisterAsDelegate(this, typeof(Game));
            this.RegisterAsDelegate(this.Window, typeof(GameWindow));

            var gameTimer = new GameTimer(this);
            this.RegisterDelegate(gameTimer);

            var gameLoopType = StartupArguments.GameLoopType;
            this.gameLoop = (IGameLoop)this.Resolve(Type.GetType(gameLoopType, true, true)!);
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
