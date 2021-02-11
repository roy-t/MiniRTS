using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Gui.Tools;
using MiniEngine.Gui.Windows;

namespace MiniEngine.Editor.Windows
{
    [Service]
    public sealed class RenderPipelineWindow : IWindow
    {
        private readonly Tool ToolSelector;
        private readonly FrameService FrameService;

        public RenderPipelineWindow(Tool toolSelector, FrameService frameService)
        {
            this.ToolSelector = toolSelector;
            this.FrameService = frameService;
        }

        public string Name => "Render Pipeline";

        public bool AllowTransparency => true;

        public void RenderContents()
        {
            Tool.BeginTable("Render Targets");
            var gBuffer = this.FrameService.GBuffer;
            this.ToolSelector.Change(ref gBuffer, new Property("GBuffer"));

            var lBuffer = this.FrameService.LBuffer;
            this.ToolSelector.Change(ref lBuffer, new Property("LBuffer"));

            var tBuffer = this.FrameService.TBuffer;
            this.ToolSelector.Change(ref tBuffer, new Property("TBuffer"));

            var pBuffer = this.FrameService.PBuffer;
            this.ToolSelector.Change(ref pBuffer, new Property("PBuffer"));
            Tool.EndTable();
        }
    }
}
