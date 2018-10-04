using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines
{
    public interface IPipelineStage
    {
        void Execute(PerspectiveCamera camera, Seconds elapsed);
    }
}
