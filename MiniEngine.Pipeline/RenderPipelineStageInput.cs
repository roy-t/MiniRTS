using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Pipeline
{
    public sealed class RenderPipelineStageInput : IPipelineInput
    {
        public void Update(PerspectiveCamera camera, Seconds elapsed, GBuffer gBuffer, string pass)
        {
            this.Camera = camera;
            this.Elapsed = elapsed;
            this.GBuffer = gBuffer;
            this.Pass = pass;
        }

        public PerspectiveCamera Camera { get; private set; }
        public Seconds Elapsed { get; private set; }
        public GBuffer GBuffer { get; private set; }
        public string Pass { get; private set; }
    }
}
