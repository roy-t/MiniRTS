using System.Collections.Generic;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Visibility
{
    public sealed class VisibilityComponent : AComponent
    {
        private readonly List<Pose> Poses;

        public VisibilityComponent(Entity entity, ICamera camera)
            : base(entity)
        {
            this.Camera = camera;
            this.Poses = new List<Pose>();
        }

        public ICamera Camera { get; set; }

        public IReadOnlyList<Pose> Visible => this.Poses;

        public void Clear() => this.Poses.Clear();

        public void AddVisiblePose(Pose pose) => this.Poses.Add(pose);
    }
}
