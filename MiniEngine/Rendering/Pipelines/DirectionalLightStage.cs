using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class DirectionalLightStage : ILightingPipelineStage
    {
        private readonly DirectionalLightSystem DirectionalLightSystem;

        public DirectionalLightStage(DirectionalLightSystem directionalLightSystem)
        {
            this.DirectionalLightSystem = directionalLightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.DirectionalLightSystem.Render(camera, gBuffer);
        }
    }
}
