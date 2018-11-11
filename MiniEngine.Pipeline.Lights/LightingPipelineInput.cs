using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Lights
{
    public sealed class LightingPipelineInput : IPipelineInput
    {
        public LightingPipelineInput(PerspectiveCamera camera, GBuffer gBuffer, string pass)
        {
            this.Camera = camera;
            this.GBuffer = gBuffer;
            this.Pass = pass;
        }

        public PerspectiveCamera Camera { get; }
        public GBuffer GBuffer { get; }
        public string Pass { get; }
    }
}
