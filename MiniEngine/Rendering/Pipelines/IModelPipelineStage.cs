using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines
{
    public interface IModelPipelineStage
    {
        void Execute(PerspectiveCamera camera, ModelRenderBatch batch, Seconds elapsed);
    }
}
