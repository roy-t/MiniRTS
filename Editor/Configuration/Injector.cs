using System.Collections.Generic;
using LightInject;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Pipeline.Debug;
using MiniEngine.Pipeline.Lights;
using MiniEngine.Pipeline.Models;
using MiniEngine.Pipeline.Particles;
using MiniEngine.Pipeline.Projectors;
using MiniEngine.Pipeline.Shadows;
using MiniEngine.Systems;
using MiniEngine.Telemetry;
using MiniEngine.UI;

namespace MiniEngine.Configuration
{
    public sealed class Injector
    {
        private readonly ServiceContainer Container;
        private readonly GameLoop GameLoop;

        public Injector(GameLoop gameLoop)
        {
            this.GameLoop = gameLoop;

            this.Container = new ServiceContainer(ContainerOptions.Default);
            this.Container.SetDefaultLifetime<PerContainerLifetime>();

            this.Compose();
        }

        public void Compose()
        {
            this.Container.RegisterInstance<Game>(this.GameLoop);
            this.Container.RegisterInstance(new SpriteBatch(this.GameLoop.GraphicsDevice));
            this.Container.RegisterInstance(this.GameLoop.GraphicsDevice);
            this.Container.RegisterInstance(this.GameLoop.Content);

            this.Container.RegisterFrom<EffectCompositionRoot>();
            this.Container.RegisterFrom<DebugCompositionRoot>();
            this.Container.RegisterFrom<LightsCompositionRoot>();
            this.Container.RegisterFrom<ModelsCompositionRoot>();
            this.Container.RegisterFrom<ParticlesCompositionRoot>();
            this.Container.RegisterFrom<ProjectorsCompositionRoot>();
            this.Container.RegisterFrom<ShadowsCompositionRoot>();
            this.Container.RegisterFrom<TelemetryCompositionRoot>();
            this.Container.RegisterFrom<SystemsCompositionRoot>();
            this.Container.RegisterFrom<UICompositionRoot>();
            this.Container.RegisterFrom<EditorCompositionRoot>();
        }

        public T Resolve<T>() => this.Container.GetInstance<T>();

        public T Resolve<T>(string name) => this.Container.GetInstance<T>(name);

        public IEnumerable<T> ResolveAll<T>() => this.Container.GetAllInstances<T>();
    }
}
