using System.IO;
using LightInject;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights
{
    public sealed class LightsCompositionRoot : ICompositionRoot
    {
        public static ContentManager Content;

        public void Compose(IServiceRegistry serviceRegistry)
        {
            this.RegisterContent<Model>(serviceRegistry, "sphere", "sphere", "Effects");

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
