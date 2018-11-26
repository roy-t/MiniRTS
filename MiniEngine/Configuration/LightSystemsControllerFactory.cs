using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;

namespace MiniEngine.Configuration
{
    public sealed class LightSystemsControllerFactory : IInstanceFactory<LightSystemsController, PerspectiveCamera>
    {
        private readonly LightsFactory LightsFactory;
        private readonly EntityController EntityController;
        private readonly KeyboardInput KeyboardInput;
        private readonly EntityCreator EntityCreator;
        private readonly EntityLinker EntityLinker;

        public LightSystemsControllerFactory(
            KeyboardInput keyboardInput,
            EntityCreator entityCreator,
            EntityController entityController,
            LightsFactory lightsFactory,
            EntityLinker entityLinker)
        {
            this.KeyboardInput = keyboardInput;
            this.EntityCreator = entityCreator;
            this.EntityController = entityController;
            this.LightsFactory = lightsFactory;
            this.EntityLinker = entityLinker;
        }

        public LightSystemsController Build(PerspectiveCamera camera)
        {
            return new LightSystemsController(
                this.KeyboardInput,
                camera,
                this.EntityCreator,
                this.EntityController,
                this.LightsFactory,
                this.EntityLinker);
        }
    }
}