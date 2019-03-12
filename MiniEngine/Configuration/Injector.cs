using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LightInject;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Input;
using MiniEngine.Rendering;
using MiniEngine.Effects;
using MiniEngine.Scenes;
using MiniEngine.Systems;
using MiniEngine.Telemetry;
using MiniEngine.Systems.Factories;
using MiniEngine.Pipeline.Lights.Factories;

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
            // Services
            this.Container.RegisterInstance(this.Device);

            // Effects            
            this.RegisterEffect<RenderEffect>("RenderEffect");
            this.RegisterEffect<CombineEffect>("CombineEffect");
            this.RegisterEffect<FxaaEffect>("FxaaEffect");
            this.RegisterEffect<BlurEffect>("BlurEffect");
            this.RegisterEffect<WeightedParticlesEffect>("WeightedParticlesEffect");
            this.RegisterEffect<AverageParticlesEffect>("AverageParticlesEffect");

            this.RegisterEffect<AmbientLightEffect>("AmbientLightEffect");
            this.RegisterEffect<DirectionalLightEffect>("DirectionalLightEffect");
            this.RegisterEffect<PointLightEffect>("PointLightEffect");
            this.RegisterEffect<ShadowCastingLightEffect>("ShadowCastingLightEffect");
            this.RegisterEffect<SunlightEffect>("SunlightEffect");

            // Textures
            this.RegisterContent<Texture2D>("NeutralMask", "neutralMask");
            this.RegisterContent<Texture2D>("NeutralNormal", "neutralNormal");
            this.RegisterContent<Texture2D>("NeutralSpecular", "neutralSpecular");

            // Primitives
            this.RegisterContent<Model>("sphere", "sphere", "Effects");

            // Systems
            this.RegisterAllOf<ISystem>();
            this.RegisterAllOf<IComponentFactory>();
            this.Container.Register<LightsFactory>();

            // Renderer
            this.Container.Register<DeferredRenderPipeline>();

            // UI
            this.Container.Register<KeyboardInput>();
            this.Container.Register<MouseInput>();

            // Entities
            this.Container.Register<EntityCreator>();
            this.Container.Register<EntityLinker>();
            this.Container.Register<EntityController>();


            // Scenes
            this.RegisterAllOf<IScene>();

            // Telemetry
#if TRACE
            this.Container.Register<IMetricServer, PrometheusMetricServer>();
            this.Container.Register<IMeterRegistry, PrometheusMeterRegistry>();
#else
            this.Container.Register<IMetricServer, NullMetricServer>();
            this.Container.Register<IMeterRegistry, NullMeterRegistry>();
#endif
        }

        public T Resolve<T>() => this.Container.GetInstance<T>();

        public T Resolve<T>(string name) => this.Container.GetInstance<T>(name);

        public IEnumerable<T> ResolveAll<T>() => this.Container.GetAllInstances<T>();

        private void RegisterContent<T>(string contentName, string named, string folder = "")
        {
            var content = this.Content.Load<T>(Path.Combine(folder, contentName));
            this.Container.RegisterInstance(typeof(T), content, named);
        }

        private void RegisterEffect<T>(string name, string folder = "Effects")
            where T : EffectWrapper, new()
        {
            this.Container.Register(
                i =>
                {
                    var wrapper = new T();
                    wrapper.Wrap(this.Content.Load<Effect>(Path.Combine(folder, name)));
                    return wrapper;
                },
                new PerRequestLifeTime());
        }

        private void RegisterAllOf<T>()
            where T : class
        {
            // TODO: use proper injection here for each referenced assembly instead of looking it up like this

            var assemblies = new List<Assembly>();
            var root = Assembly.GetExecutingAssembly();
            assemblies.Add(root);
            foreach (var assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies()) {
                assemblies.Add(Assembly.Load(assemblyName));
            }            

            foreach(var assembly in assemblies)
            {
                this.Container.RegisterAssembly(
                assembly,
                (s, _) => typeof(T).IsAssignableFrom(s) && s != typeof(T));
            }
        }
    }
}