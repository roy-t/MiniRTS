using System.Collections.Generic;
using System.Linq;

namespace MiniEngine.Pipeline.Next
{
    public sealed class PipelineStage
    {
        public PipelineStage(IReadOnlyList<ISystemBinding> systems)
        {
            this.Systems = systems;
        }

        public IReadOnlyList<ISystemBinding> Systems { get; }

        public override string ToString()
            => $"Stage: [{string.Join(", ", this.Systems.Select(s => s.ToString()))}]";
    }
}
