using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Stages
{
    public sealed class UpdateSystemStage : IPipelineStage<RenderPipelineInput>
    {
        private readonly IUpdatableSystem System;

        public UpdateSystemStage(IUpdatableSystem system)
        {
            this.System = system;
        }

        public void Execute(RenderPipelineInput input) => this.System.Update(input.Camera, input.Elapsed);
    }
}