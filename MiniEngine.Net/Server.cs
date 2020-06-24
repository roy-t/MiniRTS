using LiteNetLib;

namespace MiniEngine.Net
{
    public sealed class Server : NetworkBase
    {
        public const int DefaultServerPort = 9371;
        private const int MaxConnections = 10;

        public Server(NetworkLogger logger)
            : base(logger) { }

        public bool IsRunning { get; private set; }
        public int ConnectedPeersCount => this.NetManager.ConnectedPeersCount;

        public void Start(int port)
        {
            if (this.IsRunning)
            {
                this.Logger.Error("server: can't start server as it is already running");
                return;
            }

            this.NetManager.Start(port);
            this.IsRunning = true;
            this.Logger.Info($"server: started at ::{port}");
        }

        public void Stop()
        {
            if (!this.IsRunning)
            {
                this.Logger.Error("server: can't stop server as it is not running");
                return;
            }

            this.NetManager?.Stop(true);
            this.IsRunning = false;
            this.Logger.Info("server: stopped");
        }

        public void SendToAll(string message)
        {
            this.Writer.Put(message);
            this.NetManager.SendToAll(this.Writer, DeliveryMethod.ReliableOrdered);
            this.Writer.Reset();
        }

        public void PingClients() => this.SendToAll("Ping from server");

        public void PrintStatistics()
        {
            foreach (var peer in this.NetManager.ConnectedPeerList)
            {
                this.Logger.Info($"Peer {peer.EndPoint}\tPing: {peer.Ping}");
            }
        }

        protected override void RegisterHandlers()
        {
            this.Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                this.Logger.Info($"served: received message '{dataReader.GetString(100)}' from client {fromPeer.EndPoint}");
                dataReader.Recycle();
            };

            this.Listener.ConnectionRequestEvent += request =>
            {
                if (this.NetManager.ConnectedPeersCount < MaxConnections)
                {
                    var peer = request.AcceptIfKey(this.connectionKey);
                    if (peer != null)
                    {
                        this.Logger.Info($"server: client {peer.EndPoint} was accepted");
                    }
                    else
                    {
                        this.Logger.Error($"server: client {peer.EndPoint} was rejected due to invalid key");
                    }
                }
                else
                {
                    this.Logger.Error($"server: client was rejected, too many peers ({MaxConnections})");
                    request.Reject();
                }
            };

            this.Listener.PeerConnectedEvent += peer =>
            {
                this.Logger.Info($"server: client {peer.EndPoint} connected");
                this.Writer.Put("Hello from Server");
                peer.Send(this.Writer, DeliveryMethod.ReliableOrdered);

                this.Writer.Reset();
            };

            this.Listener.PeerDisconnectedEvent += (peer, disconnect) =>
            {
                this.Logger.Info($"server: client {peer.EndPoint} disconnected from server, {disconnect.Reason}");
            };
        }
    }
}
