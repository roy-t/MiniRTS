using MiniEngine.Configuration;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Mutators
{
    [System]
    public partial class MutatorSystem : ISystem
    {
        private readonly FrameService FrameService;

        public MutatorSystem(FrameService frameService)
        {
            this.FrameService = frameService;
        }

        public void OnSet()
        {
        }

        [ProcessAll]
        public void Update(TransformMutatorComponent mutator, TransformComponent transform)
        {
            mutator.Mutator.Update(this.FrameService.Elapsed, transform);
            transform.ChangeState.Change();
        }
    }
}
