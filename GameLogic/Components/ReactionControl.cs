using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class ReactionControl : IComponent
    {


        public ReactionControl(Entity entity, Entity[] emitters)
        {
            this.Entity = entity;
            this.Emitters = emitters;
        }

        public Entity Entity { get; }

        public Entity[] Emitters { get; }

        public Vector3 LastPosition { get; set; }

        public Vector3 Acceleration { get; set; }
        public Vector3 Velocity { get; set; }
    }
}
