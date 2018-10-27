using MiniEngine.Primitives.Cameras;
using MiniEngine.Pipeline.Lights.Systems;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class RenderShadowMapsStage : IPipelineStage
    {
        private readonly ShadowMapSystem ShadowMapSystem;

        public RenderShadowMapsStage(ShadowMapSystem shadowMapSystem)
        {
            this.ShadowMapSystem = shadowMapSystem;
        }

        public void Execute(PerspectiveCamera camera, Seconds seconds) => this.ShadowMapSystem.RenderShadowMaps();
    }
}