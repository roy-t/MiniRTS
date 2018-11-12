using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Pipeline
{
    public sealed class RenderPipelineStageInput : IPipelineInput
    {
        public RenderPipelineStageInput(GBuffer gBuffer, string pass)
        {
            this.GBuffer = gBuffer;
            this.Pass = pass;
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Camera = camera;
            this.Elapsed = elapsed;
        }

        public PerspectiveCamera Camera { get; private set; }
        public Seconds Elapsed { get; private set; }
        public GBuffer GBuffer { get; }

        public string Pass { get; }
    }
}
