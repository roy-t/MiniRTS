using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;

namespace MiniEngine.Rendering.Pipelines
{
    public interface IModelPipelineStage
    {
        void Execute(PerspectiveCamera camera, ModelRenderBatch batch);
    }
}
