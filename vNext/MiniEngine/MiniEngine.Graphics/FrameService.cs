using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics
{
    [Service]
    public sealed class FrameService
    {
        public ICamera? Camera { get; set; }
        public RenderTargetSet? RenderTargetSet { get; set; }
    }
}
