using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic.Components;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.GameLogic.Factories
{
    public sealed class FlightPlanFactory : AComponentFactory<FlightPlan>
    {
        public FlightPlanFactory(GraphicsDevice _, IComponentContainer<FlightPlan> container)
            : base(_, container) { }

        public FlightPlan Construct(Entity entity, Queue<IManeuver> maneuvers)
        {
            var flightPlan = new FlightPlan(entity, maneuvers);
            this.Container.Add(flightPlan);

            return flightPlan;
        }
    }
}
