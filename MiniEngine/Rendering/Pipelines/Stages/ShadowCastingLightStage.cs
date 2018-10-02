using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class ShadowCastingLightStage : ILightingPipelineStage
    {
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;

        public ShadowCastingLightStage(ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.ShadowCastingLightSystem.RenderLights(camera, gBuffer);
        }
    }
}
