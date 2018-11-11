using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Stages
{
    public sealed class UpdateSystemStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly IUpdatableSystem System;

        public UpdateSystemStage(IUpdatableSystem system)
        {
            this.System = system;
        }

        public void Execute(RenderPipelineStageInput input) => this.System.Update(input.Camera, input.Elapsed);
    }
}