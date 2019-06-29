using LightInject;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Controllers;
using MiniEngine.CutScene;
using MiniEngine.Input;
using MiniEngine.Pipeline.Debug;
using MiniEngine.Rendering;
using MiniEngine.Scenes;
using MiniEngine.Systems.Containers;
using MiniEngine.UI.Utilities;

namespace MiniEngine.Configuration
{
    public sealed class EditorCompositionRoot : ICompositionRoot
    {
        public static ContentManager Content;
        public static GraphicsDevice Device;

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<Waypoint>, ComponentList<Waypoint>>();
            serviceRegistry.Register<WaypointFactory>();
            serviceRegistry.Register<CutsceneSystem>();
            serviceRegistry.Register<LightsController>();

            // Renderer
            serviceRegistry.Register<PipelineBuilder>();
            serviceRegistry.Register<DeferredRenderPipeline>();

            // UI
            serviceRegistry.Register<KeyboardInput>();
            serviceRegistry.Register<MouseInput>();
            serviceRegistry.RegisterInstance(typeof(IconLibrary), new IconLibrary(Content, Device));
            serviceRegistry.Register<ComponentSearcher>();

            // Scenes
            serviceRegistry.Register<SceneBuilder>();
            serviceRegistry.Register<SceneSelector>();
            serviceRegistry.Register<IScene, SponzaScene>(nameof(SponzaScene));
            serviceRegistry.Register<IScene, LizardScene>(nameof(LizardScene));
            serviceRegistry.Register<IScene, DemoScene>(nameof(DemoScene));
        }
    }
}
