using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Bounds;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AModel : IPhysicalComponent
    {
        private Pose pose;
        private BoundingSphere boundingSphere;
        private BoundingBox boundingBox;

        private bool boundsAreDirty;

        protected AModel(Entity entity, Model model, Pose pose)
        {
            this.Entity = entity;
            this.Model = model;
            this.pose = pose;
            this.TextureScale = Vector2.One;
            this.boundsAreDirty = true;
        }

        public Entity Entity { get; }

        public Model Model { get; }

        [Editor(nameof(TextureScale))]
        public Vector2 TextureScale { get; set; }

        public BoundingSphere BoundingSphere
        {
            get
            {
                if (this.boundsAreDirty) { this.ComputeBounds(); }
                return this.boundingSphere;
            }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                if (this.boundsAreDirty) { this.ComputeBounds(); }
                return this.boundingBox;
            }
        }

        public Vector3[] Corners
        {
            get
            {
                if (this.boundsAreDirty) { this.ComputeBounds(); }
                return this.boundingBox.GetCorners();
            }
        }

        public IconType Icon => IconType.Model;

        public Vector3 Position => this.pose.Translation;

        public Matrix[] SkinTransforms { get; set; }

        public bool HasAnimations => this.SkinTransforms != null;

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
            => this.pose.Rotate(yaw, pitch, roll);

        public void Move(Vector3 position)
        {
            this.pose.Move(position);
            this.boundsAreDirty = true;
        }

        public void SetScale(Vector3 scale)
        {
            this.pose.SetScale(scale);
            this.boundsAreDirty = true;
        }

        public void SetScale(float scale) => this.SetScale(Vector3.One * scale);

        private void ComputeBounds()
        {
            if (this.boundsAreDirty)
            {
                this.Model.ComputeExtremes(this.pose.Matrix, out var min, out var max);
                this.boundingBox = new BoundingBox(min, max);
                this.boundingSphere = BoundingSphere.CreateFromBoundingBox(this.boundingBox);

                this.boundsAreDirty = false;
            }
        }
    }
}
