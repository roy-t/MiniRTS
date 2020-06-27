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

        public NetState NetState => this.State.NetState;

        public void Render(PerspectiveCamera camera)
        {

            if (ImGui.BeginMenu("Net"))
            {
                this.NetState.ShowNetWindow = this.Toggle("Toggle Details", this.NetState.ShowNetWindow);

                ImGui.Separator();

                this.NetState.AutoStartServer = this.Toggle("Auto Start Server", this.NetState.AutoStartServer);
                this.NetState.AutoStartClient = this.Toggle("Auto Start Client", this.NetState.AutoStartClient);

                ImGui.EndMenu();
            }
        }


        private bool Toggle(string name, bool currentValue)
        {
            ImGui.Checkbox(name, ref currentValue);
            return currentValue;
        }
    }
}
