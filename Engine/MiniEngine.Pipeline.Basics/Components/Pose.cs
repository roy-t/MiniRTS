using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Basics.Components
{
    public sealed class Pose : IComponent
    {
        private Vector3 position, scale, origin;
        private float yaw, pitch, roll;

        public Pose(Entity entity, Vector3 position, Vector3 scale, float yaw, float pitch, float roll)
        {
            this.Entity = entity;
            this.Origin = Vector3.Zero;
            this.Position = position;
            this.Scale = scale;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Roll = roll;

            this.Matrix = Recompute(this.Origin, position, scale, yaw, pitch, roll);
        }

        public Entity Entity { get; }

        public Matrix Matrix { get; private set; }

        [Editor(nameof(Origin))]
        public Vector3 Origin
        {
            get => this.origin;
            set => this.SetOrigin(value);
        }

        [Editor(nameof(Yaw))]
        public float Yaw
        {
            get => this.yaw;
            set => this.Rotate(value, this.pitch, this.roll);
        }

        [Editor(nameof(Pitch))]
        public float Pitch
        {
            get => this.pitch;
            set => this.Rotate(this.yaw, value, this.roll);
        }

        [Editor(nameof(Roll))]
        public float Roll
        {
            get => this.roll;
            set => this.Rotate(this.yaw, this.pitch, value);
        }

        [Editor(nameof(Position))]
        public Vector3 Position
        {
            get => this.position;
            set => this.Move(value);
        }

        [Editor(nameof(Scale))]
        public Vector3 Scale
        {
            get => this.scale;
            set => this.SetScale(value);
        }

        public Vector3 GetForward()
        {
            var rotation = Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, this.roll);
            return Vector3.Transform(Vector3.Forward, rotation);
        }

        public void Rotate(float yaw, float pitch, float roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
            this.Matrix = Recompute(this.origin, this.position, this.scale, this.yaw, this.pitch, this.roll);
        }

        public void Move(Vector3 position)
        {
            this.position = position;
            this.Matrix = Recompute(this.origin, this.position, this.scale, this.yaw, this.pitch, this.roll);
        }

        public void PlaceAtOffset(Offset offset, Pose other)
        {
            var rotation = Matrix.CreateFromYawPitchRoll(other.Yaw, other.Pitch, other.Roll);

            this.Yaw = MathHelper.WrapAngle(other.Yaw + offset.Yaw);
            this.Pitch = MathHelper.WrapAngle(other.Pitch + offset.Pitch);
            this.Roll = MathHelper.WrapAngle(other.Roll + offset.Roll);

            this.position = other.position + Vector3.Transform(offset.Position, rotation);
            this.Matrix = Recompute(this.origin, this.position, this.scale, this.yaw, this.pitch, this.roll);
        }

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;
            this.Matrix = Recompute(this.origin, this.position, this.scale, this.yaw, this.pitch, this.roll);
        }

        public void SetScale(float scale) => this.SetScale(Vector3.One * scale);

        public void SetOrigin(Vector3 origin)
        {
            this.origin = origin;
            this.Matrix = Recompute(this.origin, this.position, this.scale, this.yaw, this.pitch, this.roll);
        }

        private static Matrix Recompute(Vector3 origin, Vector3 position, Vector3 scale, float yaw, float pitch, float roll)
        {
            var moveToCenter = Matrix.CreateTranslation(-origin);
            var rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            var size = Matrix.CreateScale(scale);
            var translation = Matrix.CreateTranslation(position);

            return size * moveToCenter * rotation * translation;
        }
    }
}
