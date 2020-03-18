using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Bounds;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using ModelExtension;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AModel : IPhysicalComponent
    {
        private Pose pose;

        protected AModel(Entity entity, Model model, Pose pose)
        {
            this.Entity = entity;
            this.Model = model;
            this.pose = pose;
            this.Animation = new IdentityAnimation();
            this.TextureScale = Vector2.One;
            this.ComputeBounds();

            this.HasAnimations = this.Model.Tag is SkinningData;
        }

        public Entity Entity { get; }

        public Model Model { get; }

        [Editor(nameof(TextureScale))]
        public Vector2 TextureScale { get; set; }

        public AAnimation Animation { get; set; }

        public BoundingSphere BoundingSphere { get; private set; }

        public BoundingBox BoundingBox { get; private set; }

        public Vector3[] Corners => this.BoundingBox.GetCorners();

        public IconType Icon => IconType.Model;

        public Vector3 Position => this.pose.Translation;

        public bool HasAnimations { get; }

        [Editor(nameof(Origin))]
        public Vector3 Origin { get => this.pose.Origin; set => this.pose.SetOrigin(value); }

        [Editor(nameof(Yaw))]
        public float Yaw { get => this.pose.Yaw; set => this.Rotate(value, this.pose.Pitch, this.pose.Roll); }

        [Editor(nameof(Pitch))]
        public float Pitch { get => this.pose.Pitch; set => this.Rotate(this.pose.Yaw, value, this.pose.Roll); }

        [Editor(nameof(Roll))]
        public float Roll { get => this.pose.Roll; set => this.Rotate(this.pose.Yaw, this.pose.Pitch, value); }

        [Editor(nameof(Translation))]
        public Vector3 Translation { get => this.pose.Translation; set => this.Move(value); }

        [Editor(nameof(Scale))]
        public Vector3 Scale { get => this.pose.Scale; set => this.SetScale(value); }


        public Matrix WorldMatrix => this.pose.Matrix;


        public Pose Pose
        {
            get => this.pose;
            set
            {
                this.pose = value;
                this.ComputeBounds();
            }
        }

        public void Rotate(float yaw, float pitch, float roll)
        {
            this.pose.Rotate(yaw, pitch, roll);
            this.ComputeBounds();
        }

        public void Move(Vector3 position)
        {
            this.pose.Move(position);
            this.ComputeBounds();
        }

        public void SetScale(Vector3 scale)
        {
            this.pose.SetScale(scale);
            this.ComputeBounds();
        }

        public void SetScale(float scale) => this.SetScale(Vector3.One * scale);

        private void ComputeBounds()
        {
            this.BoundingSphere = this.Model.ComputeBoundingSphere(this.pose.Matrix);
            this.BoundingBox = this.Model.ComputeBoundingBox(this.pose.Matrix);
        }
    }
}
