using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Lights
{
    public interface ILightingPipelineStage
    {
        void Execute(PerspectiveCamera camera, GBuffer gBuffer);
    }
}