using Microsoft.Xna.Framework;
using MiniEngine.Editor.Configuration;
using MiniEngine.Systems.Injection;
using MiniEngine.Systems.Pipeline;

namespace MiniEngine.Editor
{
    public class GameLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private readonly Register RegisterDelegate;
        private readonly RenderPipelineBuilder RenderPipelineBuilder;

        private ParallelPipeline? renderPipeline;

        public GameLoop(Register registerDelegate, RenderPipelineBuilder renderPipelineBuilder)
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.RegisterDelegate = registerDelegate;
            this.RenderPipelineBuilder = renderPipelineBuilder;
        }

        protected override void LoadContent()
        {
            this.RegisterDelegate(this.Graphics.GraphicsDevice);
            this.RegisterDelegate(this.Content);

            this.renderPipeline = this.RenderPipelineBuilder.Build();
        }

        protected override void UnloadContent()
        {
            this.renderPipeline!.Stop();
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            this.Window.Title = $"Editor :: {gameTime.ElapsedGameTime.TotalMilliseconds:F2}ms";
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.renderPipeline!.Run();
            this.renderPipeline!.Wait();
            base.Draw(gameTime);
        }
    }
}
