using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Visibility
{
    public sealed class FakeSpatialPartitioningStructure
    {
        private record Entry(Entity Entity, GeometryModel Model, Matrix Transform);

        private readonly List<Entry> Entries;

        public FakeSpatialPartitioningStructure()
        {
            this.Entries = new List<Entry>();
        }

        public void Add(Entity entity, GeometryModel model, Matrix transform)
        {
            var entry = new Entry(entity, model, transform);
            this.Entries.Add(entry);
        }

        public void Update(Entity entity, Matrix transform)
        {
            for (var i = this.Entries.Count - 1; i >= 0; i--)
            {
                var pose = this.Entries[i];
                if (pose.Entity == entity)
                {
                    this.Entries[i] = new Entry(entity, pose.Model, transform);
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

        public void GetVisibleGeometry(ICamera camera, IList<Pose> outVisible)
        {
            var frustum = new BoundingFrustum(camera.ViewProjection);
            for (var i = 0; i < this.Entries.Count; i++)
            {
                var entry = this.Entries[i];

                outVisible.Add(new Pose(entry.Model, entry.Transform));
                // TODO: how to fix for sunlight?
                //var bounds = entry.Model.Bounds.Transform(entry.Transform);
                //if (frustum.Contains(bounds) != ContainmentType.Disjoint)
                //{
                //    outVisible.Add(new Pose(entry.Model, entry.Transform));
                //}
            }
        }
    }
}
