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

        [ProcessNew]
        public void ProcessNew(TransformComponent transform)
            => transform.ProcessQueue();

        [ProcessChanged]
        public void ProcessQueues(TransformComponent transform)
            => transform.ProcessQueue();
    }
}
