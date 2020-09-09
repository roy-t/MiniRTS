using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Models;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Pipeline;

namespace MiniEngine.Editor
{
    public class GameLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private readonly Register RegisterDelegate;
        private readonly Resolve ResolveDelegate;
        private readonly RenderPipelineBuilder RenderPipelineBuilder;

        private ParallelPipeline? renderPipeline;

        public GameLoop(Register registerDelegate, Resolve resolveDelegate, RenderPipelineBuilder renderPipelineBuilder)
        {
            this.Graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.RegisterDelegate = registerDelegate;
            this.ResolveDelegate = resolveDelegate;
            this.RenderPipelineBuilder = renderPipelineBuilder;
        }

        protected override void LoadContent()
        {
            this.RegisterDelegate(this.Graphics.GraphicsDevice);
            this.RegisterDelegate(this.Content);

            this.renderPipeline = this.RenderPipelineBuilder.Build();

            var container = (ComponentContainer<ModelComponent>)this.ResolveDelegate(typeof(ComponentContainer<ModelComponent>));
            container.Add(new ModelComponent(new Systems.Entity(1), null));


            var container2 = (ComponentContainer<BodyComponent>)this.ResolveDelegate(typeof(ComponentContainer<BodyComponent>));
            container2.Add(new BodyComponent(new Systems.Entity(1)));
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
