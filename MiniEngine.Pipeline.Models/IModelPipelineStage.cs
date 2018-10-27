using MiniEngine.Primitives.Cameras;
using MiniEngine.Pipeline.Models.Batches;

namespace MiniEngine.Pipeline.Models
{
    public interface IModelPipelineStage
    {
        void Execute(PerspectiveCamera camera, ModelRenderBatch batch);
    }
}