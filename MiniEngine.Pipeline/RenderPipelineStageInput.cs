using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Pipeline
{
    public sealed class RenderPipelineStageInput : IPipelineInput
    {
        public RenderPipelineStageInput(PerspectiveCamera camera, Seconds elapsed)
        {
            this.Camera = camera;
            this.Elapsed = elapsed;
        }

        public PerspectiveCamera Camera { get; }
        public Seconds Elapsed { get; }

        public string Pass => "render";
    }
}
