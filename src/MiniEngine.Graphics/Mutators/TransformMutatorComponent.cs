using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Mutators
{
    public sealed class TransformMutatorComponent : AComponent
    {
        public TransformMutatorComponent(Entity entity, IMutatorFunction<Matrix> mutator)
            : base(entity)
        {
            this.Mutator = mutator;
        }

        public IMutatorFunction<Matrix> Mutator { get; }
    }
}
