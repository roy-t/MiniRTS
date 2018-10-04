using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class RenderShadowMapsStage : IPipelineStage
    {
        private readonly ShadowMapSystem ShadowMapSystem;

        public RenderShadowMapsStage(ShadowMapSystem shadowMapSystem)
        {
            this.ShadowMapSystem = shadowMapSystem;
        }

        public void Execute(PerspectiveCamera camera, Seconds _)
        {
            this.ShadowMapSystem.RenderShadowMaps();
        }
    }
}
