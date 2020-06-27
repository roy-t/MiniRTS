using System.Net;
using LiteNetLib;

namespace MiniEngine.Net
{
    public sealed class Client : NetworkBase
    {
        public Client(NetworkLogger logger)
            : base(logger) { }

        public void Connect(IPEndPoint endPoint)
        {
            this.NetManager.Start();
            this.NetManager.Connect(endPoint, this.connectionKey);
            this.IsConnected = true;
        }

        public bool IsConnected;

        public void Disconnect()
        {
            this.NetManager.Stop();
            this.IsConnected = false;
        }

        protected override void RegisterHandlers()
        {


            this.Listener.PeerConnectedEvent += peer =>
            {
                this.Logger.Info("client: connected to server");
            };

            this.Listener.PeerDisconnectedEvent += (peer, disconnect) =>
            {
                this.Logger.Warn($"client: disconnected from server, {disconnect.Reason}");
            };
        }

        public void PingServer()
        {
            var command = NetCommand.Create(1, 1, "Ping!");
            this.SendCommand(command, this.NetManager.FirstPeer);
        }

        protected override void OnCommandReceived(NetCommand command, NetPeer peer)
        {
            this.Logger.Info($"client: received command {command.CommandId}:{command.Payload} from {command.Player} at {peer.EndPoint}");
        }
    }
}
