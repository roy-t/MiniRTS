using System.Net;

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
            this.Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                this.Logger.Info($"client: received message '{dataReader.GetString(100)}' from server {fromPeer.EndPoint}");
                dataReader.Recycle();
            };

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
            this.Writer.Put("Ping from client");
            this.NetManager.FirstPeer.Send(this.Writer, LiteNetLib.DeliveryMethod.ReliableOrdered);
            this.Writer.Reset();
        }
    }
}
