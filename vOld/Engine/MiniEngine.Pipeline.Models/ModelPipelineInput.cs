using MiniEngine.Pipeline.Models.Batches;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Models
{
    public sealed class ModelPipelineInput : IPipelineInput
    {
        public void Update(PerspectiveCamera camera, ModelRenderBatch batch, GBuffer gBuffer, Pass pass)
        {
            this.Camera = camera;
            this.Batch = batch;
            this.GBuffer = gBuffer;
            this.Pass = pass;
        }

        public PerspectiveCamera Camera { get; private set; }
        public ModelRenderBatch Batch { get; private set; }
        public GBuffer GBuffer { get; private set; }
        public Pass Pass { get; private set; }
    }
}
