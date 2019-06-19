
using MiniEngine.Pipeline.Shadows.Systems;

namespace MiniEngine.Pipeline.Shadows.Stages
{
    public sealed class RenderShadowMapsStage : IPipelineStage<ShadowPipelineInput>
    {
        private readonly ShadowMapSystem ShadowMapSystem;

        public RenderShadowMapsStage(ShadowMapSystem shadowMapSystem)
        {
            this.ShadowMapSystem = shadowMapSystem;
        }

        public void Execute(ShadowPipelineInput input) => this.ShadowMapSystem.RenderShadowMaps(input.GBuffer);
    }
}