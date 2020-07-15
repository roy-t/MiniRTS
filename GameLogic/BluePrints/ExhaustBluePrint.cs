using Microsoft.Xna.Framework;

namespace MiniEngine.GameLogic.BluePrints
{
    public sealed class ExhaustBluePrint
    {
        public ExhaustBluePrint(Vector3 offset, float yaw, float pitch, float roll)
        {
            this.Offset = offset;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Roll = roll;
        }

        public Vector3 Offset { get; }
        public float Yaw { get; }
        public float Pitch { get; }
        public float Roll { get; }
    }
}
