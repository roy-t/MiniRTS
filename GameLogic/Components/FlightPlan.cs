using System.Collections.Generic;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
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

        [Editor(nameof(ManeuverCount))]
        public int ManeuverCount => this.Maneuvers.Count;

        public Queue<IManeuver> Maneuvers { get; }
    }
}
