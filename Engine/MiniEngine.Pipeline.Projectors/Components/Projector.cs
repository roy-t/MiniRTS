using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Projectors.Components
{
    public sealed class Projector : IComponent
    {
        private const float Epsilon = 0.001f;

        public Projector(Entity entity, Texture2D texture, Texture2D mask, Color tint, Vector3 position, Vector3 lookAt, Meters minDistance, Meters maxDistance)
        {
            this.Entity = entity;
            this.Texture = texture;
            this.Mask = mask;

            this.Tint = tint;

            this.ViewPoint = new PerspectiveCamera(1);
            this.ViewPoint.Move(position, lookAt);

            this.SetMinDistance(minDistance);
            this.SetMaxDistance(maxDistance);
        }

        public Entity Entity { get; }

        [Editor(nameof(Texture))]
        public Texture2D Texture { get; }

        [Editor(nameof(Mask))]
        public Texture2D Mask { get; }

        [Editor(nameof(Tint))]
        public Color Tint { get; set; }        

        [Editor(nameof(MinDistance), nameof(SetMinDistance), Epsilon, float.MaxValue)]
        public float MinDistance { get; private set; }

        [Editor(nameof(MaxDistance), nameof(SetMaxDistance), Epsilon, float.MaxValue)]
        public float MaxDistance { get; private set; }

        [Editor(nameof(ViewPoint))]
        public PerspectiveCamera ViewPoint { get; set; }

        [Boundary(BoundaryType.Frustum)]
        public BoundingFrustum Bounds => this.ViewPoint.Frustum;

        [Icon(IconType.Camera)]
        public Vector3 Position => this.ViewPoint.Position;

        [Icon(IconType.LookAt)]
        public Vector3 LookAt => this.ViewPoint.LookAt;

        public void SetMinDistance(float distance)
        {
            distance = MathHelper.Clamp(distance, Epsilon, this.MaxDistance - Epsilon);

            this.MinDistance = distance;
            this.ViewPoint.SetPlanes(this.MinDistance, this.MaxDistance);
        }

        public void SetMaxDistance(float distance)
        {
            distance = MathHelper.Clamp(distance, this.MinDistance + Epsilon, float.MaxValue);

            this.MaxDistance = distance;
            this.ViewPoint.SetPlanes(this.MinDistance, this.MaxDistance);
        }        
    }
}
