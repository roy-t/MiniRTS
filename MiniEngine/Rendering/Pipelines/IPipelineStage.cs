using MiniEngine.Rendering.Cameras;

namespace MiniEngine.Rendering.Pipelines
{
    public interface IPipelineStage
    {
        void Execute(PerspectiveCamera camera);
    }
}
