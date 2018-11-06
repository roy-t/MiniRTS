using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Shadows.Stages
{
    public sealed class RenderShadowsStage : IPipelineStage
    {
        private readonly ShadowPipeline ShadowPipeline;

        public RenderShadowsStage(ShadowPipeline shadowPipeline)
        {
            this.ShadowPipeline = shadowPipeline;
        }

        public void Execute(PerspectiveCamera camera, Seconds elapsed) => this.ShadowPipeline.Execute();
    }
}
