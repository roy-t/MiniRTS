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
        }

        public ICamera Camera { get; set; }
    }
}
