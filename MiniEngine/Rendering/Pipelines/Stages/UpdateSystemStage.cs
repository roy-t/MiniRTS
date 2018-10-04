using MiniEngine.Rendering.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class UpdateSystemStage : IPipelineStage
    {
        private readonly IUpdatableSystem System;

        public UpdateSystemStage(IUpdatableSystem system)
        {
            this.System = system;
        }

        public void Execute(PerspectiveCamera camera, Seconds seconds)
        {
            this.System.Update(camera, seconds);
        }
    }
}