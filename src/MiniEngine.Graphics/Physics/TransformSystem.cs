using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Physics
{
    [System]
    public partial class TransformSystem : ISystem
    {
        private readonly FrameService FrameService;

        public TransformSystem(FrameService frameService)
        {
            this.FrameService = frameService;
        }

        public void OnSet() { }

        [ProcessChanged]
        public void ProcessQueues(TransformComponent transform)
            => transform.ProcessQueue();
    }
}
