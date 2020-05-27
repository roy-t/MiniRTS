using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Utilities;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public class TranslationManeuver : IManeuver
    {
        private readonly Pose Pose;
        private Seconds accumulator;

        public TranslationManeuver(Pose pose, Vector3 targetPosition, float metersPerSecond)
        {
            this.Pose = pose;
            this.StartPosition = pose.Position;
            this.TargetPosition = targetPosition;
            this.ETA = Vector3.Distance(this.StartPosition, this.TargetPosition) / metersPerSecond;
        }

        public void Update(Seconds elapsed)
        {
            var progress = Easings.Interpolate(this.accumulator / this.ETA, Easings.Functions.QuadraticEaseInOut);
            this.Pose.Position = Vector3.Lerp(this.StartPosition, this.TargetPosition, progress);

            this.accumulator += elapsed;
            this.Completed = this.accumulator >= this.ETA;
        }

        public Vector3 StartPosition { get; }
        public Vector3 TargetPosition { get; }
        public Seconds ETA { get; set; }

        public bool Completed { get; private set; }

    }
}
