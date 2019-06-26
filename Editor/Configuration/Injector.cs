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


            // Services
            

//            // Effects            
//            //this.RegisterEffect<RenderEffect>("RenderEffect");
//            //this.RegisterEffect<CombineEffect>("CombineEffect");
//            //this.RegisterEffect<FxaaEffect>("FxaaEffect");
//            //this.RegisterEffect<BlurEffect>("BlurEffect");
//            //this.RegisterEffect<WeightedParticlesEffect>("WeightedParticlesEffect");
//            //this.RegisterEffect<AverageParticlesEffect>("AverageParticlesEffect");
//            //this.RegisterEffect<AdditiveParticlesEffect>("AdditiveParticlesEffect");
//            //this.RegisterEffect<ColorEffect>("ColorEffect");
//            //this.RegisterEffect<TextureEffect>("TextureEffect");
//            //this.RegisterEffect<UIEffect>("UIEffect");
//            //this.RegisterEffect<AmbientLightEffect>("AmbientLightEffect");
//            //this.RegisterEffect<DirectionalLightEffect>("DirectionalLightEffect");
//            //this.RegisterEffect<PointLightEffect>("PointLightEffect");
//            //this.RegisterEffect<ShadowCastingLightEffect>("ShadowCastingLightEffect");
//            //this.RegisterEffect<SunlightEffect>("SunlightEffect");
//            //this.RegisterEffect<ProjectorEffect>("ProjectorEffect");           

//            // Primitives
//            this.RegisterContent<Model>("sphere", "sphere", "Effects");

//            // Systems
//            this.RegisterAllOf<ISystem>();
//            //this.RegisterAllOf<IComponentContainer>();
//            this.RegisterAllOf<IComponentFactory>();
//            this.Container.Register<LightsFactory>();

//            // Renderer
//            this.Container.Register<PipelineBuilder>();
//            this.Container.Register<DeferredRenderPipeline>();            

//            // UI
//            this.Container.Register<KeyboardInput>();
//            this.Container.Register<MouseInput>();
//            this.Container.RegisterInstance(typeof(IconLibrary), new IconLibrary(this.Content, this.Device));
//            this.Container.Register<ComponentSearcher>();

//            // Entities
//            this.Container.Register<EntityCreator>();
//            this.Container.Register<EntityLinker>();
//            this.Container.Register<EntityController>();
//            this.Container.Register<EntityManager>();

//            // Scenes
//            this.Container.Register<SceneBuilder>();
//            this.RegisterAllOf<IScene>();

//            // Telemetry
//#if TRACE
//            this.Container.Register<IMetricServer, PrometheusMetricServer>();
//            this.Container.Register<IMeterRegistry, PrometheusMeterRegistry>();
//#else
//            this.Container.Register<IMetricServer, NullMetricServer>();
//            this.Container.Register<IMeterRegistry, NullMeterRegistry>();
//#endif
        }

        public T Resolve<T>() => this.Container.GetInstance<T>();

        public T Resolve<T>(string name) => this.Container.GetInstance<T>(name);

        public IEnumerable<T> ResolveAll<T>() => this.Container.GetAllInstances<T>();

        //private void RegisterContent<T>(string contentName, string named, string folder = "")
        //{
        //    var content = this.Content.Load<T>(Path.Combine(folder, contentName));
        //    this.Container.RegisterInstance(typeof(T), content, named);
        //}

        //private void RegisterEffect<T>(string name, string folder = "Effects")
        //    where T : EffectWrapper, new()
        //{
        //    this.Container.Register(
        //        i =>
        //        {
        //            var wrapper = new T();
        //            wrapper.Wrap(this.Content.Load<Effect>(Path.Combine(folder, name)));
        //            return wrapper;
        //        },
        //        new PerRequestLifeTime());
        //}        

        //private void RegisterAllOf<T>()
        //    where T : class
        //{
        //    // TODO: use proper injection here for each referenced assembly instead of looking it up like this

        //    var assemblies = new List<Assembly>();
        //    var root = Assembly.GetExecutingAssembly();
        //    assemblies.Add(root);
        //    foreach (var assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
        //    {
        //        assemblies.Add(Assembly.Load(assemblyName));
        //    }

        //    foreach (var assembly in assemblies)
        //    {
        //        this.Container.RegisterAssembly(
        //        assembly,
        //        (s, _) => typeof(T).IsAssignableFrom(s) && s != typeof(T));
        //    }
        //}
    }
}