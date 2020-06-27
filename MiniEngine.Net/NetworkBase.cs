using System;
using LiteNetLib;
using LiteNetLib.Utils;

namespace MiniEngine.Net
{
    public abstract class NetworkBase
    {
        protected readonly NetworkLogger Logger;
        protected readonly EventBasedNetListener Listener;
        protected readonly NetManager NetManager;
        protected readonly NetDataWriter Writer;

        protected readonly NetPacketProcessor processor;

        protected string connectionKey = "x";

        public NetworkBase(NetworkLogger logger)
        {
            this.Listener = new EventBasedNetListener();
            this.NetManager = new NetManager(this.Listener)
            {
                NatPunchEnabled = true,
                DisconnectTimeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds,
            };

            this.processor = new NetPacketProcessor();
            this.processor.RegisterNestedType<Player>();
            this.processor.SubscribeReusable<NetCommand, NetPeer>(this.OnCommandReceived);

            this.Writer = new NetDataWriter();
            this.Logger = logger;

            this.RegisterHandlers();

            this.Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                this.processor.ReadAllPackets(dataReader, fromPeer);
            };
        }

        public void SetConnectionKey(string key) => this.connectionKey = key;

        public void Update() => this.NetManager.PollEvents();

        protected abstract void OnCommandReceived(NetCommand command, NetPeer peer);

        protected void SendCommand(NetCommand command, NetPeer peer, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
            => peer.Send(this.processor.Write(command), deliveryMethod);

        protected abstract void RegisterHandlers();
    }
}
