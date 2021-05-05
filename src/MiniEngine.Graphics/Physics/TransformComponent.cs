using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Physics
{
    public sealed class TransformComponent : AComponent
    {


        public TransformComponent(Entity entity)
           : this(entity, Vector3.Zero, Vector3.One, Quaternion.Identity) { }


        public TransformComponent(Entity entity, Vector3 position)
            : this(entity, position, Vector3.One, Quaternion.Identity) { }

        public TransformComponent(Entity entity, Vector3 position, float scale)
            : this(entity, position, Vector3.One * scale, Quaternion.Identity) { }

        public TransformComponent(Entity entity, Vector3 position, Vector3 scale)
            : this(entity, position, scale, Quaternion.Identity) { }

        public TransformComponent(Entity entity, Vector3 position, Vector3 scale, Quaternion rotation, Vector3 origin = default)
            : base(entity)
        {
            this.Transform = new Transform(position, scale, rotation, origin);
        }

        public Transform Transform { get; }

        public Matrix Matrix => this.Transform.Matrix;
        public Quaternion Rotation => this.Transform.Rotation;
        public Vector3 Origin => this.Transform.Origin;
        public Vector3 Position => this.Transform.Position;
        public Vector3 Forward => this.Transform.Forward;
        public Vector3 Scale => this.Transform.Scale;

        public void MoveTo(Vector3 position) => this.Transform.MoveTo(position);
        public void SetScale(float scale) => this.Transform.SetScale(scale);
        public void SetScale(Vector3 scale) => this.Transform.SetScale(scale);
        public void SetOrigin(Vector3 origin) => this.Transform.SetOrigin(origin);
        public void SetRotation(Quaternion rotation) => this.Transform.SetRotation(rotation);
        public void ApplyRotation(Quaternion rotation) => this.Transform.ApplyRotation(rotation);
        public void FaceTarget(Vector3 target) => this.Transform.FaceTarget(target);
        public void FaceTargetConstrained(Vector3 target, Vector3 up) => this.Transform.FaceTargetConstrained(target, up);
    }
}
