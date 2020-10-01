using System;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Graphics
{
    [Service]
    public sealed class FrameService
    {
        public FrameService()
        {
            this.Camera = new PerspectiveCamera(1.0f);
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public ICamera Camera { get; set; }
    }
}
