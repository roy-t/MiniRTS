using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines
{
    public interface ILightingPipelineStage
    {
        void Execute(PerspectiveCamera camera, GBuffer gBuffer, Seconds elapsed);
    }
}
