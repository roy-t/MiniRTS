using MiniEngine.Pipeline.Models;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class RenderLightsStage : IPipelineStage<ModelPipelineInput>
    { 
        private readonly LightingPipeline LightingPipeline;

        public RenderLightsStage(LightingPipeline lightingPipeline)
        {
            this.LightingPipeline = lightingPipeline;
        }

        public void Execute(ModelPipelineInput input) => this.LightingPipeline.Execute(new LightingPipelineInput(input.Camera, input.GBuffer, "model"));
    }
}