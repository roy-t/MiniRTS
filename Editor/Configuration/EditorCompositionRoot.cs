using LightInject;
using MiniEngine.Controllers;
using MiniEngine.CutScene;
using MiniEngine.Input;
using MiniEngine.Pipeline.Debug;
using MiniEngine.Rendering;
using MiniEngine.Scenes;
using MiniEngine.Systems.Containers;
using MiniEngine.UI;
using MiniEngine.UI.Utilities;

namespace MiniEngine.Configuration
{
    public sealed class EditorCompositionRoot : ICompositionRoot
    {

        public void Compose(IServiceRegistry serviceRegistry)
        {
            serviceRegistry.Register<IComponentContainer<Waypoint>, ComponentContainer<Waypoint>>();
            serviceRegistry.Register<WaypointFactory>();
            serviceRegistry.Register<CutsceneSystem>();
            serviceRegistry.Register<CameraController>();
            serviceRegistry.Register<LightsController>();

            // Renderer
            serviceRegistry.Register<PipelineBuilder>();
            serviceRegistry.Register<DeferredRenderPipeline>();
            serviceRegistry.Register<RenderTargetDescriber>();
            serviceRegistry.Register<RenderPipelineBuilder>();
            serviceRegistry.Register<AnimationPipelineBuilder>();

            // UI
            serviceRegistry.Register<KeyboardInput>();
            serviceRegistry.Register<MouseInput>();
            serviceRegistry.Register<IconLibrary>();
            serviceRegistry.Register<ComponentSearcher>();

            // Menus
            serviceRegistry.Register<UIManager>();
            serviceRegistry.Register<IMenu, FileMenu>("0");
            serviceRegistry.Register<IMenu, EntityMenu>("1");
            serviceRegistry.Register<IMenu, CreateMenu>("2");
            serviceRegistry.Register<IMenu, RenderingMenu>("4");
            serviceRegistry.Register<IMenu, DebugMenu>("5");
            serviceRegistry.Register<EntityWindow>();

            // Scenes
            serviceRegistry.Register<SkyboxBuilder>();
            serviceRegistry.Register<SceneBuilder>();
            serviceRegistry.Register<SceneSelector>();
            serviceRegistry.Register<IScene, SponzaScene>(nameof(SponzaScene));
            serviceRegistry.Register<IScene, DemoScene>(nameof(DemoScene));
            serviceRegistry.Register<IScene, FlightScene>(nameof(FlightScene));
            serviceRegistry.Register<IScene, EmptyScene>(nameof(EmptyScene));
        }
    }
}
