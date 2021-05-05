using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Visibility
{
    public sealed class FakeSpatialPartitioningStructure
    {
        private readonly List<Pose> Entries;

        public FakeSpatialPartitioningStructure()
        {
            this.Entries = new List<Pose>();
        }

        public void Add(Entity entity, float radius, Transform transform, IRenderService renderService)
        {
            var entry = new Pose(entity, new BoundingSphere(Vector3.Zero, radius), transform, renderService);
            this.Entries.Add(entry);
        }

        public void Update(Entity entity, float radius, Transform transform)
        {
            for (var i = this.Entries.Count - 1; i >= 0; i--)
            {
                var pose = this.Entries[i];
                if (pose.Entity == entity)
                {
                    this.Entries[i].Transform = transform;
                    this.Entries[i].Bounds = new BoundingSphere(Vector3.Zero, radius);
                    return;
                }
            }
        }

        public void Remove(Entity entity)
        {
            for (var i = this.Entries.Count - 1; i >= 0; i--)
            {
                if (this.Entries[i].Entity == entity)
                {
                    this.Entries.RemoveAt(i);
                    return;
                }
            }
        }

        public void GetVisibleEntities(PerspectiveCamera camera, IList<Pose> outVisible)
        {
            var frustum = new BoundingFrustum(camera.ViewProjection);
            for (var i = 0; i < this.Entries.Count; i++)
            {
                var entry = this.Entries[i];
                var bounds = entry.Bounds.Transform(entry.Transform.Matrix);
                if (frustum.Contains(bounds) != ContainmentType.Disjoint)
                {
                    outVisible.Add(entry);
                }
            }
        }
    }
}
