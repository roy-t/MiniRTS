using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.GameLogic.Factories
{
    public sealed class ReactionControlFactory : AComponentFactory<ReactionControl>
    {
        public ReactionControlFactory(GraphicsDevice _, IComponentContainer<ReactionControl> container)
            : base(_, container) { }

        public ReactionControl Construct(Entity target, params Entity[] emitters)
        {
            if (emitters.Length == 0)
            {
                throw new ArgumentException("Should pass at least one emitter entity");
            }

            var rcs = new ReactionControl(target, emitters);
            this.Container.Add(rcs);
            return rcs;
        }
    }
}
