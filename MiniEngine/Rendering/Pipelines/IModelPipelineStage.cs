using MiniEngine.Rendering.Batches;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Rendering.Pipelines
{
    public interface IModelPipelineStage
    {
        void Execute(PerspectiveCamera camera, ModelRenderBatch batch);
    }
}