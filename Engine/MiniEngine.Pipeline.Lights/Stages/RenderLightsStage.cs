using MiniEngine.Pipeline.Models;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class RenderLightsStage : IPipelineStage<ModelPipelineInput>
    {
        private readonly LightingPipeline LightingPipeline;
        private readonly LightingPipelineInput Input;

        public RenderLightsStage(LightingPipeline lightingPipeline)
        {
            this.LightingPipeline = lightingPipeline;
            this.Input = new LightingPipelineInput();
        }

        public void Execute(ModelPipelineInput input)
        {
            this.Input.Update(input.Camera, input.GBuffer, input.Pass);
            this.LightingPipeline.Execute(this.Input, "lights");
        }
    }
}