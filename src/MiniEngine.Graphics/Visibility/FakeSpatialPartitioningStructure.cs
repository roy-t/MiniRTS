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

        public void Add(Entity entity, BoundingSphere bounds, Transform transform, IRenderService renderService)
        {
            var entry = new Pose(entity, bounds, transform, renderService);
            this.Entries.Add(entry);
        }

        public void Update(Entity entity, Transform transform)
        {
            for (var i = this.Entries.Count - 1; i >= 0; i--)
            {
                var pose = this.Entries[i];
                if (pose.Entity == entity)
                {
                    this.Entries[i].Transform = transform;
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
                outVisible.Add(entry);
                // TODO: how to fix for sunlight?
                // TODO: how to fix for instancing?

                //var bounds = entry.Model.Bounds.Transform(entry.Transform);
                //if (frustum.Contains(bounds) != ContainmentType.Disjoint)
                //{
                //    outVisible.Add(new Pose(entry.Model, entry.Transform));
                //}
            }
        }
    }
}
