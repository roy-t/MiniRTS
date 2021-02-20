using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics
{
    public sealed class TransformComponent : AComponent
    {
        public TransformComponent(Entity entity)
            : base(entity)
        {
            this.Matrix = Matrix.Identity;
        }

        public TransformComponent(Entity entity, Matrix matrix)
            : base(entity)
        {
            this.Matrix = matrix;
        }

        public TransformComponent(Entity entity, Vector3 position, Vector3 scale, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
            : this(entity, Combine(position, scale, yaw, pitch, roll)) { }

        public TransformComponent(Entity entity, Vector3 position, Vector3 scale, Vector3 origin, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
            : this(entity, Combine(position, scale, origin, yaw, pitch, roll)) { }

        public Matrix Matrix { get; set; }

        public static Matrix Combine(Vector3 position, Vector3 scale, Vector3 origin, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
        {
            var moveToCenter = Matrix.CreateTranslation(-origin);
            var size = Matrix.CreateScale(scale);
            var translation = Matrix.CreateTranslation(position);

            var rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            return size * moveToCenter * rotationMatrix * translation;
        }

        public static Matrix Combine(Vector3 position, Vector3 scale, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
            => Combine(position, scale, Vector3.Zero, yaw, pitch, roll);

    }
}
