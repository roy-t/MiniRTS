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

            this.RecomputeMatrices();
        }

        public Entity Entity { get; }

        public Matrix Matrix { get; private set; }

        public Matrix RotationMatrix { get; private set; }

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
            this.RecomputeMatrices();
        }

        public void Move(Vector3 position)
        {
            this.position = position;
            this.RecomputeMatrices();
        }

        public void PlaceAtOffset(Offset offset, Pose other)
        {
            this.position = other.position + Vector3.Transform(offset.Position, other.RotationMatrix);
            this.yaw = offset.Yaw;
            this.pitch = offset.Pitch;
            this.roll = offset.Roll;
            this.RecomputeMatrices(other.RotationMatrix);
        }

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;
            this.RecomputeMatrices();
        }

        public void SetScale(float scale) => this.SetScale(Vector3.One * scale);

        public void SetOrigin(Vector3 origin)
        {
            this.origin = origin;
            this.RecomputeMatrices();
        }

        private void RecomputeMatrices(Matrix additionalRotation)
        {
            var moveToCenter = Matrix.CreateTranslation(-this.origin);
            var size = Matrix.CreateScale(this.scale);
            var translation = Matrix.CreateTranslation(this.position);

            this.RotationMatrix = Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, this.roll) * additionalRotation;
            this.Matrix = size * moveToCenter * this.RotationMatrix * translation;
        }

        private void RecomputeMatrices()
            => this.RecomputeMatrices(Matrix.Identity);
    }
}
