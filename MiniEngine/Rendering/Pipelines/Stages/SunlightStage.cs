using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class SunlightStage : ILightingPipelineStage
    {
        private readonly SunlightSystem SunlightSystem;

        public SunlightStage(SunlightSystem sunlightSystem)
        {
            this.SunlightSystem = sunlightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.SunlightSystem.RenderLights(camera, gBuffer);
        }
    }
}
