using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Stages
{
    public sealed class UpdateSystemStage : IPipelineStage
    {
        private readonly IUpdatableSystem System;

        public UpdateSystemStage(IUpdatableSystem system)
        {
            this.System = system;
        }

        public void Execute(PerspectiveCamera camera, Seconds seconds) => this.System.Update(camera, seconds);
    }
}