using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines
{
    public interface ILightingPipelineStage
    {
        void Execute(PerspectiveCamera camera, GBuffer gBuffer);
    }
}
