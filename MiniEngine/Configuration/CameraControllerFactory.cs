using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Configuration
{
    public sealed class CameraControllerFactory : IInstanceFactory<CameraController, PerspectiveCamera>
    {
        private readonly KeyboardInput KeyboardInput;
        private readonly MouseInput MouseInput;

        public CameraControllerFactory(KeyboardInput keyboardInput, MouseInput mouseInput)
        {
            this.KeyboardInput = keyboardInput;
            this.MouseInput = mouseInput;
        }

        public CameraController Build(PerspectiveCamera camera) => new CameraController(this.KeyboardInput, this.MouseInput, camera);
    }
}