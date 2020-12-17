using System.Collections.Generic;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Camera
{
    public sealed class CameraComponent : AComponent
    {
        public CameraComponent(Entity entity, ICamera camera)
            : base(entity)
        {
            this.Camera = camera;
            this.InView = new List<Pose>();
        }

        public ICamera Camera { get; }

        public List<Pose> InView { get; }
    }
}
