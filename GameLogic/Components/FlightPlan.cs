using System.Collections.Generic;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.GameLogic.Components
{
    public sealed class FlightPlan : IComponent
    {
        public FlightPlan(Entity entity, Queue<IManeuver> maneuvers)
        {
            this.Entity = entity;
            this.Maneuvers = maneuvers;
        }

        public Entity Entity { get; }

        public bool IsCompleted => this.Maneuvers.Count == 0;

        public Queue<IManeuver> Maneuvers { get; }
    }
}
