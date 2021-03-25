using MiniEngine.Graphics.Physics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Mutators
{
    public sealed class TransformMutatorComponent : AComponent
    {
        public TransformMutatorComponent(Entity entity, IMutatorFunction<TransformComponent> mutator)
            : base(entity)
        {
            this.Mutator = mutator;
        }

        public IMutatorFunction<TransformComponent> Mutator { get; }
    }
}
