using System.Collections.Generic;
using MiniEngine.GameLogic.Components;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Systems
{
    public sealed class FlightPlanSystem : IUpdatableSystem
    {
        private readonly IComponentContainer<FlightPlan> FlightPlans;
        private readonly IComponentContainer<Pose> Poses;
        private readonly List<Entity> Completed;

        public FlightPlanSystem(
            IComponentContainer<FlightPlan> flightPlans,
            IComponentContainer<Pose> poses)
        {
            this.FlightPlans = flightPlans;
            this.Poses = poses;

            this.Completed = new List<Entity>();
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            for (var i = 0; i < this.FlightPlans.Count; i++)
            {
                var flightPlan = this.FlightPlans[i];
                var pose = this.Poses.Get(flightPlan.Entity);

                if (flightPlan.Maneuvers.Count > 0)
                {
                    var maneuver = flightPlan.Maneuvers.Peek();
                    maneuver.Update(pose, elapsed);

                    if (maneuver.Completed)
                    {
                        flightPlan.Maneuvers.Dequeue();
                    }
                }
                else
                {
                    this.Completed.Add(flightPlan.Entity);
                }
            }

            for (var i = 0; i < this.Completed.Count; i++)
            {
                var entity = this.Completed[i];
                this.FlightPlans.Remove(entity);
            }
            this.Completed.Clear();
        }
    }
}
