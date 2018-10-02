using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class PointLightStage : ILightingPipelineStage
    {
        private readonly PointLightSystem PointLightSystem;

        public PointLightStage(PointLightSystem pointLightSystem)
        {
            this.PointLightSystem = pointLightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.PointLightSystem.Render(camera, gBuffer);
        }
    }
}
