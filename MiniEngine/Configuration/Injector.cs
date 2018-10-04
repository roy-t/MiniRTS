using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LightInject;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Input;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Effects;
using MiniEngine.Scenes;
using MiniEngine.Systems;
using MiniEngine.Utilities;

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

            Compose();
        }

        public void Compose()
        {
            RegisterAllOf<IInstanceFactory>();

            // Services
            this.Container.RegisterInstance(this.Device);

            // Effects            
            RegisterEffect<RenderEffect>("RenderEffect");
            RegisterEffect<CombineEffect>("CombineEffect");
            RegisterEffect<PostProcessEffect>("PostProcessEffect");
            RegisterEffect<CopyEffect>("CopyEffect");

            RegisterContent<Effect>("ClearEffect", "clearEffect", "Effects");
            RegisterContent<Effect>("PostProcessOutlineEffect", "postProcessOutlineEffect", "Effects");
            RegisterContent<Effect>("DirectionalLightEffect", "directionalLightEffect", "Effects");
            RegisterContent<Effect>("PointLightEffect", "pointLightEffect", "Effects");
            RegisterContent<Effect>("ShadowCastingLightEffect", "shadowCastingLightEffect", "Effects");
            RegisterContent<Effect>("SunlightEffect", "sunlightEffect", "Effects");

            // Textures
            RegisterContent<Texture2D>("NeutralMask", "neutralMask");
            RegisterContent<Texture2D>("NeutralNormal", "neutralNormal");
            RegisterContent<Texture2D>("NeutralSpecular", "neutralSpecular");

            // Primitives
            RegisterContent<Model>("sphere", "sphere", "Effects");

            // Systems
            RegisterAllOf<ISystem>();

            // Renderer
            this.Container.Register<DeferredRenderPipeline>();

            // Input
            this.Container.Register<KeyboardInput>();
            this.Container.Register<MouseInput>();

            // Controllers
            this.Container.Register<EntityController>();
            this.Container.Register<DebugController>();

            // Scenes
            RegisterAllOf<IScene>();
        }

        public T Resolve<T>()
        {
            return this.Container.GetInstance<T>();
        }

        public T Resolve<T>(string name)
        {
            return this.Container.GetInstance<T>(name);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return this.Container.GetAllInstances<T>();
        }

        private void RegisterContent<T>(string contentName, string named, string folder = "")
        {
            var content = this.Content.Load<T>(Path.Combine(folder, contentName));
            this.Container.RegisterInstance(typeof (T), content, named);
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
            this.Container.RegisterAssembly(
                Assembly.GetExecutingAssembly(),
                (s, _) => typeof (T).IsAssignableFrom(s) && s != typeof (T));
        }      
    }
}