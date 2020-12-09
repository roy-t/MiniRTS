using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Visibility
{
    public sealed class FakeSpatialPartitioningStructure
    {
        private record Entry(Entity Entity, GeometryData Geometry, Material Material, Matrix Transform);

        private readonly List<Entry> Entries;

        public FakeSpatialPartitioningStructure()
        {
            this.Entries = new List<Entry>();
        }

        public void Add(Entity entity, GeometryData geometry, Material material, Matrix transform)
        {
            var entry = new Entry(entity, geometry, material, transform);
            this.Entries.Add(entry);
        }

        public void Update(Entity entity, Matrix transform)
        {
            for (var i = this.Entries.Count - 1; i >= 0; i++)
            {
                var pose = this.Entries[i];
                if (pose.Entity == entity)
                {
                    this.Entries[i] = new Entry(entity, pose.Geometry, pose.Material, transform);
                    return;
                }
            }
        }

        public void Remove(Entity entity)
        {
            for (var i = this.Entries.Count - 1; i >= 0; i++)
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

                var bounds = entry.Geometry.Bounds.Transform(entry.Transform);
                if (frustum.Contains(bounds) != ContainmentType.Disjoint)
                {
                    outVisible.Add(new Pose(entry.Geometry, entry.Material, entry.Transform));
                }
            }
        }
    }
}
