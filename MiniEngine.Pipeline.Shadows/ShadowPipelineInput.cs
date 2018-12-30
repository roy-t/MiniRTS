using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Shadows
{
    public sealed class ShadowPipelineInput : IPipelineInput
    {
        public void Update(GBuffer gBuffer, PerspectiveCamera camera, string pass)
        {
            this.GBuffer = gBuffer;
            this.Camera = camera;
            this.Pass = pass;
        }

        public GBuffer GBuffer { get; private set; }
        public PerspectiveCamera Camera { get; private set; }
        public string Pass { get; private set; }
    }
}
