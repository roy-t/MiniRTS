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
        private readonly List<Pose> AlwaysVisible;

        public FakeSpatialPartitioningStructure()
        {
            this.Entries = new List<Pose>();
            this.AlwaysVisible = new List<Pose>();
        }

        public void Add(Entity entity, BoundingSphere bounds, Transform transform, IRenderService renderService, bool alwaysVisible = false)
        {
            var entry = new Pose(entity, bounds, transform, renderService);
            if (alwaysVisible)
            {
                this.AlwaysVisible.Add(entry);
            }
            else
            {
                this.Entries.Add(entry);
            }
        }

        public void Update(Entity entity, BoundingSphere bounds, Transform transform)
        {
            for (var i = this.Entries.Count - 1; i >= 0; i--)
            {
                var pose = this.Entries[i];
                if (pose.Entity == entity)
                {
                    this.Entries[i].Transform = transform;
                    this.Entries[i].Bounds = bounds;
                    return;
                }
            }

            for (var i = this.AlwaysVisible.Count - 1; i >= 0; i--)
            {
                var pose = this.AlwaysVisible[i];
                if (pose.Entity == entity)
                {
                    this.AlwaysVisible[i].Transform = transform;
                    this.AlwaysVisible[i].Bounds = bounds;
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

            for (var i = this.AlwaysVisible.Count - 1; i >= 0; i--)
            {
                if (this.AlwaysVisible[i].Entity == entity)
                {
                    this.AlwaysVisible.RemoveAt(i);
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

                // TODO: how to fix for sunlight?

                var bounds = entry.Bounds.Transform(entry.Transform.Matrix);
                if (frustum.Contains(bounds) != ContainmentType.Disjoint)
                {
                    outVisible.Add(entry);
                }
            }

            for (var i = 0; i < this.AlwaysVisible.Count; i++)
            {
                var entry = this.AlwaysVisible[i];
                outVisible.Add(entry);
            }
        }
    }
}
