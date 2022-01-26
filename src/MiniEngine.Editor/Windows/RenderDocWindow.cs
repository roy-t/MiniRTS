using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using MiniEngine.Gui.Windows;

namespace MiniEngine.Editor.Windows
{
    [Service]
    public sealed class RenderDocWindow : IWindow
    {
        private readonly RenderDoc? RenderDoc;

        public RenderDocWindow(Resolve resolve)
        {
            try
            {
                this.RenderDoc = (RenderDoc)resolve(typeof(RenderDoc));
            }
            catch { }
        }

        public string Name => "RenderDoc";
        public bool AllowTransparency => true;

        public void RenderContents()
        {
            if (this.RenderDoc == null)
            {
                ImGui.Text("RenderDoc has been disabled");
            }
            else
            {
                if (ImGui.Button("Capture"))
                {
                    this.RenderDoc.TriggerCapture();
                }

                if (this.RenderDoc.GetNumCaptures() > 0 && ImGui.Button("Open Last Capture"))
                {
                    var path = this.RenderDoc.GetCapture(this.RenderDoc.GetNumCaptures() - 1) ?? string.Empty;
                    this.RenderDoc.LaunchReplayUI(path);
                }
            }
        }
    }
}
