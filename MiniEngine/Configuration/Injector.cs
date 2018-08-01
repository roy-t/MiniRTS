using LightInject;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Input;
using MiniEngine.Rendering;
using MiniEngine.Scenes;
using MiniEngine.Systems;
using System.Collections.Generic;
using System.Reflection;

namespace MiniEngine.Configuration
{
    public sealed class Injector
    {
        private readonly ServiceContainer Container;
        private readonly ContentManager Content;

        public Injector(GraphicsDevice device, ContentManager content)
        {
            this.Content = content;

            this.Container = new ServiceContainer(ContainerOptions.Default);
            this.Container.SetDefaultLifetime<PerContainerLifetime>();
            RegisterAllOf<IInstanceFactory>();

            // Services
            this.Container.RegisterInstance(device);

            // Effects
            RegisterContent<Effect>("clearEffect");
            RegisterContent<Effect>("combineEffect");
            RegisterContent<Effect>("postProcessEffect");

            RegisterContent<Effect>("directionalLightEffect");
            RegisterContent<Effect>("pointLightEffect");
            RegisterContent<Effect>("shadowMapEffect");
            RegisterContent<Effect>("shadowCastingLightEffect");
            RegisterContent<Effect>("cascadingShadowMapEffect");
            RegisterContent<Effect>("sunlightEffect");

            // Primitives
            RegisterContent<Model>("sphere");
            
            // Systems
            RegisterAllOf<ISystem>();         
            
            // Renderer
            this.Container.Register<DeferredRenderer>();

            // Input
            this.Container.Register<KeyboardInput>();
            this.Container.Register<MouseInput>();

            // Controllers
            this.Container.Register<EntityController>();
            
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

        private void RegisterContent<T>(string name)
        {
            var content = this.Content.Load<T>(name);
            this.Container.RegisterInstance(typeof (T), content, name);
        }

        private void RegisterAllOf<T>()
            where T : class 
        {
            this.Container.RegisterAssembly(
                Assembly.GetExecutingAssembly(),
                (s, _) => typeof(T).IsAssignableFrom(s) && s != typeof(T));
        }        
    }
}
