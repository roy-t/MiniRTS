using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Particles
{
    public sealed class ParticlePipelineInput : IPipelineInput
    {
        public void Update(PerspectiveCamera camera, GBuffer gBuffer, string pass)
        {
            this.Camera = camera;
            this.GBuffer = gBuffer;
            this.Pass = pass;
        }

        public PerspectiveCamera Camera { get; private set; }
        public GBuffer GBuffer { get; private set; }
        public string Pass { get; private set; }
    }
}
