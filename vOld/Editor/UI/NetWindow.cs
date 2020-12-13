using System.Net;
using System.Numerics;
using ImGuiNET;
using MiniEngine.Net;
using MiniEngine.UI.State;

namespace MiniEngine.UI
{
    public sealed class NetWindow
    {
        private readonly Server Server;
        private readonly Client Client;
        private readonly NetworkLogger Logger;

        public NetWindow(Server server, Client client, NetworkLogger logger)
        {
            this.Server = server;
            this.Client = client;
            this.Logger = logger;
        }

        public UIState State { get; set; }
        public NetState NetState => this.State.NetState;

        public void Render()
        {
            if (ImGui.Begin("Networking Details", ref this.NetState.ShowNetWindow, ImGuiWindowFlags.NoCollapse))
            {
                if (ImGui.Button("Start Server"))
                {
                    this.Server.Start(Server.DefaultServerPort);
                }

                ImGui.SameLine();
                if (ImGui.Button("Stop Server"))
                {
                    this.Server.Stop();
                }

                ImGui.SameLine();
                if (ImGui.Button("Ping Clients"))
                {
                    this.Server.PingClients();
                }

                if (ImGui.Button("Start Client"))
                {
                    this.Client.Connect(new IPEndPoint(IPAddress.Loopback, Server.DefaultServerPort));
                }

                ImGui.SameLine();
                if (ImGui.Button("Stop Client"))
                {
                    this.Client.Disconnect();
                }

                ImGui.SameLine();
                if (ImGui.Button("Ping Server"))
                {
                    this.Client.PingServer();
                }

                if (ImGui.Button("Clear"))
                {
                    this.Logger.Clear();
                }
                ImGui.Separator();

                if (ImGui.Button("Print Statistics"))
                {
                    this.Server.PrintStatistics();
                }
                ImGui.Text($"Connected peers:\t{this.Server.ConnectedPeersCount}");

                ImGui.Separator();

                ImGui.BeginChild("scrolling", new Vector2(0, 0), false, ImGuiWindowFlags.HorizontalScrollbar);
                ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
                ImGui.TextUnformatted(this.Logger.BuildString());

                ImGui.PopStyleVar();

                if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                {
                    ImGui.SetScrollHereY(1.0f);
                }

                ImGui.EndChild();
                ImGui.End();
            }
        }
    }
}
