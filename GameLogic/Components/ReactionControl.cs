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
            this.EmitterReactionRange = 0.5f;
        }

        public Entity Entity { get; }

        public Entity[] Emitters { get; }

        /// <summary>
        /// When an emitter paired to this RCS should react
        /// 1.0 react if the acceleration vector perfectly aligns with the emitter vector
        /// 0.0 react if the acceleration vector makes a <= 90 degree angle with the emitter vector        
        /// </summary>
        public float EmitterReactionRange { get; set; }

        public Vector3 LastPosition { get; set; }

        public Vector3 Acceleration { get; set; }
        public Vector3 Velocity { get; set; }
    }
}
