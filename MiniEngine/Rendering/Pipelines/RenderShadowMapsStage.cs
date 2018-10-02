using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class RenderShadowMapsStage : IPipelineStage
    {
        private readonly ShadowMapSystem ShadowMapSystem;

        public RenderShadowMapsStage(ShadowMapSystem shadowMapSystem)
        {
            this.ShadowMapSystem = shadowMapSystem;
        }

        public void Execute(PerspectiveCamera camera)
        {
            this.ShadowMapSystem.RenderShadowMaps();
        }
    }
}
