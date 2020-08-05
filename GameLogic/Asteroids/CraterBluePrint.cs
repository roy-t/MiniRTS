using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Generators;
using MiniEngine.Systems.Annotations;

namespace GameLogic.Asteroids
{
    public sealed class CraterBluePrint
    {
        public CraterBluePrint()
        {
            this.Pitch = 0;
            this.Yaw = 0;
            this.Radius = 0.15f;
            this.FloorHeight = 0.9f;
            this.Smoothness = 0.5f;
        }

        [Editor(nameof(Pitch), nameof(Pitch), -MathHelper.Pi, MathHelper.Pi)]
        public float Pitch { get; set; }

        [Editor(nameof(Yaw), nameof(Yaw), 0.0f, MathHelper.TwoPi)]
        public float Yaw { get; set; }

        [Editor(nameof(Radius), nameof(Radius), 0.01f, 1.0f)]
        public float Radius { get; set; }

        [Editor(nameof(FloorHeight), nameof(FloorHeight), 0.01f, 1.0f)]
        public float FloorHeight { get; set; }

        [Editor(nameof(Smoothness), nameof(Smoothness), 0.01f, 1.0f)]
        public float Smoothness { get; set; }

        public Crater ToCrater()
        {
            var rotation = Matrix.CreateFromYawPitchRoll(this.Yaw, this.Pitch, 0.0f);
            var position = Vector3.Transform(Vector3.Forward, rotation);
            return new Crater()
            {
                floor = this.FloorHeight,
                position = position,
                radius = this.Radius,
                smoothness = this.Smoothness
            };
        }
    }
}
