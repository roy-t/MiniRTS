
using MiniEngine.Pipeline.Shadows.Systems;

namespace MiniEngine.Pipeline.Shadows.Stages
{
    public sealed class RenderShadowMapsStage : IShadowPipelineStage
    {
        private readonly ShadowMapSystem ShadowMapSystem;

        public RenderShadowMapsStage(ShadowMapSystem shadowMapSystem)
        {
            this.ShadowMapSystem = shadowMapSystem;
        }

        public void Execute() => this.ShadowMapSystem.RenderShadowMaps();
    }
}