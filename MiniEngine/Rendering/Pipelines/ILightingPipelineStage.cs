using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;

namespace MiniEngine.Rendering.Pipelines
{
    public interface ILightingPipelineStage
    {
        void Execute(PerspectiveCamera camera, GBuffer gBuffer);
    }
}