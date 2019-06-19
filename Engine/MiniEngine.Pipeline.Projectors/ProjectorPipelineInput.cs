using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Projectors
{
    public sealed class ProjectorPipelineInput : IPipelineInput
    {
        public void Update(GBuffer gBuffer, PerspectiveCamera camera, Pass pass)
        {
            this.GBuffer = gBuffer;
            this.Camera = camera;
            this.Pass = pass;
        }

        public GBuffer GBuffer { get; private set; }
        public PerspectiveCamera Camera { get; private set; }
        public Pass Pass { get; private set; }
    }
}
