using System.IO;
using LightInject;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Lights.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Lights
{
    public sealed class LightsCompositionRoot : ICompositionRoot
    {
        public static ContentManager Content;

        public void Compose(IServiceRegistry serviceRegistry)
        {
            this.RegisterContent<Microsoft.Xna.Framework.Graphics.Model>(serviceRegistry, "sphere", "sphere", "Effects");

            serviceRegistry.Register<IComponentContainer<AmbientLight>, ComponentList<AmbientLight>>();
            serviceRegistry.Register<IComponentContainer<DirectionalLight>, ComponentList<DirectionalLight>>();
            serviceRegistry.Register<IComponentContainer<PointLight>, ComponentList<PointLight>>();            
            serviceRegistry.Register<IComponentContainer<ShadowCastingLight>, ComponentList<ShadowCastingLight>>();
            serviceRegistry.Register<IComponentContainer<Sunlight>, ComponentList<Sunlight>>();            


            serviceRegistry.Register<LightsFactory>();

            serviceRegistry.Register<AmbientLightFactory>();
            serviceRegistry.Register<DirectionalLightFactory>();
            
            serviceRegistry.Register<PointLightFactory>();
            serviceRegistry.Register<ShadowCastingLightFactory>();
            serviceRegistry.Register<SunlightFactory>();

            serviceRegistry.Register<AmbientLightSystem>();
            serviceRegistry.Register<DirectionalLightSystem>();
            serviceRegistry.Register<PointLightSystem>();
            serviceRegistry.Register<ShadowCastingLightSystem>();
            serviceRegistry.Register<SunlightSystem>();
        }

        private void RegisterContent<T>(IServiceRegistry serviceRegistry, string contentName, string named, string folder = "")
        {
            var content = Content.Load<T>(Path.Combine(folder, contentName));
            serviceRegistry.RegisterInstance(typeof(T), content, named);
        }
    }
}
