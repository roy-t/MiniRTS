using System.Collections.Generic;
using LightInject;
using Microsoft.Xna.Framework.Content;
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

namespace MiniEngine.Configuration
{
    public sealed class Injector
    {
        private readonly ServiceContainer Container;
        private readonly ContentManager Content;
        private readonly GraphicsDevice Device;        

        public Injector(GraphicsDevice device, ContentManager content)
        {
            this.Content = content;
            this.Device = device;

            this.Container = new ServiceContainer(ContainerOptions.Default);
            this.Container.SetDefaultLifetime<PerContainerLifetime>();

            this.Compose();
        }

        public void Compose()
        {
            // TODO: register factories so that composition roots do not need static members
            this.Container.RegisterInstance(this.Device);
            this.Container.RegisterInstance(this.Content);

            EffectCompositionRoot.Content = this.Content;
            this.Container.RegisterFrom<EffectCompositionRoot>();
            this.Container.RegisterFrom<DebugCompositionRoot>();

            LightsCompositionRoot.Content = this.Content;
            this.Container.RegisterFrom<LightsCompositionRoot>();

            this.Container.RegisterFrom<ModelsCompositionRoot>();
            this.Container.RegisterFrom<ParticlesCompositionRoot>();
            this.Container.RegisterFrom<ProjectorsCompositionRoot>();
            this.Container.RegisterFrom<ShadowsCompositionRoot>();
            this.Container.RegisterFrom<TelemetryCompositionRoot>();
            this.Container.RegisterFrom<SystemsCompositionRoot>();

            EditorCompositionRoot.Content = this.Content;
            EditorCompositionRoot.Device = this.Device;
            this.Container.RegisterFrom<EditorCompositionRoot>();
        }

        public T Resolve<T>() => this.Container.GetInstance<T>();

        public T Resolve<T>(string name) => this.Container.GetInstance<T>(name);

        public IEnumerable<T> ResolveAll<T>() => this.Container.GetAllInstances<T>();       
    }
}