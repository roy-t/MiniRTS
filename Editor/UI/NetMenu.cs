using ImGuiNET;
using MiniEngine.Net;
using MiniEngine.Primitives.Cameras;
using MiniEngine.UI.State;

namespace MiniEngine.UI
{
    public sealed class NetMenu : IMenu
    {
        private readonly Server Server;

        public NetMenu(Server server)
        {
            this.Server = server;
        }

        public UIState State { get; set; }

        public void Render(PerspectiveCamera camera)
        {

            if (ImGui.BeginMenu("Net"))
            {
                //if (this.Server.IsRunning)
                //{
                //    if (ImGui.MenuItem("Stop Server"))
                //    {
                //        this.Server.Stop();
                //    }
                //}
                //else
                //{
                //    if (ImGui.MenuItem("Start Server"))
                //    {
                //        this.Server.Start();
                //    }
                //}

                var show = this.State.NetState.ShowNetWindow;
                ImGui.Checkbox("Toggle Details", ref show);
                this.State.NetState.ShowNetWindow = show;

                ImGui.EndMenu();
            }
        }
    }
}
