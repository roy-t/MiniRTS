using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class UpdateCascadesStage : IPipelineStage
    {
        private readonly SunlightSystem SunlightSystem;

        public UpdateCascadesStage(SunlightSystem sunlightSystem)
        {
            this.SunlightSystem = sunlightSystem;
        }

        public void Execute(PerspectiveCamera camera)
        {
            this.SunlightSystem.Update(camera);
        }
    }
}
