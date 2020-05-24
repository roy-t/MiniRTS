using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Systems
{
    public sealed class AccelerometerSystem : IUpdatableSystem
    {
        private readonly IComponentContainer<Accelerometer> Accelerometers;
        private readonly IComponentContainer<Pose> Poses;

        public AccelerometerSystem(IComponentContainer<Accelerometer> accelerometers, IComponentContainer<Pose> poses)
        {
            this.Accelerometers = accelerometers;
            this.Poses = poses;
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            for (var i = 0; i < this.Accelerometers.Count; i++)
            {
                var accelerometer = this.Accelerometers[i];
                var newPosition = this.Poses.Get(accelerometer.Entity).Position;

                accelerometer.UpdateAccelerationAndVelocity(newPosition, elapsed);
            }
        }
    }
}
