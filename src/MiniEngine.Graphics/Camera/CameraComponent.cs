using System.Collections.Generic;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Camera
{
    public sealed class CameraComponent : AComponent
    {
        public CameraComponent(Entity entity, PerspectiveCamera camera)
            : base(entity)
        {
            this.Camera = camera;
            this.InView = new List<Pose>();
        }

        public PerspectiveCamera Camera { get; }

        public List<Pose> InView { get; }
    }
}
