using MiniEngine.Pipeline.Models.Batches;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Models
{
    public sealed class ModelPipelineInput : IPipelineInput
    {
        public ModelPipelineInput(PerspectiveCamera camera, ModelRenderBatch batch, GBuffer gBuffer, string pass)
        {
            this.Camera = camera;
            this.Batch = batch;
            this.GBuffer = gBuffer;
            this.Pass = pass;
        }

        public PerspectiveCamera Camera { get; }
        public ModelRenderBatch Batch { get; }
        public GBuffer GBuffer { get; }
        public string Pass { get; }
    }
}
