using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Shadows
{
    public sealed class ShadowPipelineInput : IPipelineInput
    {
        public void Update(PerspectiveCamera camera, string pass)
        {
            this.Camera = camera;
            this.Pass = pass;
        }

        public PerspectiveCamera Camera { get; private set; }
        public string Pass { get; private set; }
    }
}
