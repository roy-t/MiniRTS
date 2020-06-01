using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class BurnRetroBurnManeuver : IManeuver
    {
        private Pose pose;
        private readonly Vector3 targetPosition;
        private float maxLinearAcceleration;
        private float maxAngularAcceleration;

        private float distance;
        private float rotateAfter;

        private Vector3 direction;

        private float velocity;
        private float distanceTraveled;

        private SubManeuver subManeuver;


        public BurnRetroBurnManeuver(Pose pose, Vector3 targetPosition, float maxLinearAcceleration, float maxAngularAcceleration)
        {
            this.pose = pose;
            this.targetPosition = targetPosition;
            this.maxLinearAcceleration = maxLinearAcceleration;
            this.maxAngularAcceleration = maxAngularAcceleration;

            this.direction = pose.GetForward();

            this.distance = Vector3.Distance(targetPosition, this.pose.Position);
            this.rotateAfter = this.distance / 2.0f;

            this.subManeuver = SubManeuver.ProgradeBurn;
        }

        // TODO: https://math.stackexchange.com/questions/3701612/moving-a-rocket-between-two-points-on-a-straight-line-when-to-rotate-from-progr

        public bool Completed { get; private set; }

        public void Update(Seconds elapsed)
        {
            switch (this.subManeuver)
            {
                case SubManeuver.ProgradeBurn:
                    this.PerformProgradeBurn(elapsed);
                    break;
                case SubManeuver.Rotate:
                    this.Rotate(elapsed);
                    break;
                case SubManeuver.RetrogradeBurn:
                    this.PerformRetroGradeBurn(elapsed);
                    break;
                default:
                    this.velocity = 0.0f;
                    this.pose.Move(this.targetPosition);
                    this.Completed = true;
                    break;
            }

            if (this.velocity != 0.0f)
            {
                var change = this.velocity * elapsed;
                this.distanceTraveled += change;
                this.pose.Move(this.pose.Position + (this.direction * change));
            }
        }

        private void PerformProgradeBurn(Seconds elapsed)
        {
            if (this.distanceTraveled >= this.rotateAfter)
            {
                this.subManeuver = SubManeuver.Rotate;
            }
            else
            {
                this.velocity += this.maxLinearAcceleration * elapsed;
            }
        }

        private void Rotate(Seconds elapsed)
        {
            var yaw = AngleMath.YawFromVector(-this.direction);
            var pitch = AngleMath.PitchFromVector(-this.direction);

            this.pose.Rotate(yaw, pitch, this.pose.Roll);
            this.subManeuver = SubManeuver.RetrogradeBurn;
        }

        private void PerformRetroGradeBurn(Seconds elapsed)
        {
            if (this.distanceTraveled >= this.distance)
            {
                this.subManeuver = SubManeuver.Completed;
            }
            else
            {
                this.velocity -= this.maxLinearAcceleration * elapsed;
            }
        }

        private enum SubManeuver
        {
            ProgradeBurn,
            Rotate,
            RetrogradeBurn,
            Completed
        }
    }
}