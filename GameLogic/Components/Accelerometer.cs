using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class Accelerometer : IComponent
    {
        private Vector3 position;

        public Accelerometer(Entity entity)
        {
            this.Entity = entity;
        }

        public Entity Entity { get; }

        public Vector3 Acceleration { get; private set; }
        public Vector3 Velocity { get; private set; }

        public void UpdateAccelerationAndVelocity(Vector3 newPosition, Seconds elapsed)
        {
            var newVelocity = (newPosition - this.position) / elapsed;
            this.Acceleration = newVelocity - this.Velocity;

            this.Velocity = newVelocity;
            this.position = newPosition;
        }
    }
}
