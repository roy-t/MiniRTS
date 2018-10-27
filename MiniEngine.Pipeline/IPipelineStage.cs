using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Pipeline
{
    public interface IPipelineStage
    {
        void Execute(PerspectiveCamera camera, Seconds seconds);
    }
}