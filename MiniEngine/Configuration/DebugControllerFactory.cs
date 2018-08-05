using MiniEngine.Input;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Utilities;

namespace MiniEngine.Configuration
{
    public sealed class DebugControllerFactory : IInstanceFactory<DebugController, PerspectiveCamera>
    {
        private readonly KeyboardInput KeyboardInput;
        private readonly CameraControllerFactory CameraControllerFactory;
        private readonly LightSystemsControllerFactory LightSystemsControllerFactory;        

        public DebugControllerFactory(KeyboardInput keyboardInput, CameraControllerFactory cameraControllerFactory,
                                      LightSystemsControllerFactory lightSystemsControllerFactory)
        {
            this.KeyboardInput = keyboardInput;
            this.CameraControllerFactory = cameraControllerFactory;
            this.LightSystemsControllerFactory = lightSystemsControllerFactory;
        }

        public DebugController Build(PerspectiveCamera camera)
        {
            return new DebugController(
                this.KeyboardInput,
                camera,
                this.CameraControllerFactory,
                this.LightSystemsControllerFactory);
        }
    }
}
