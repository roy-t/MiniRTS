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

        protected string connectionKey = "x";

        public NetworkBase(NetworkLogger logger)
        {
            this.Listener = new EventBasedNetListener();
            this.NetManager = new NetManager(this.Listener)
            {
                NatPunchEnabled = true,
                DisconnectTimeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds,
            };

            this.Writer = new NetDataWriter();
            this.Logger = logger;

            this.RegisterHandlers();
        }

        public void SetConnectionKey(string key) => this.connectionKey = key;

        public void Update() => this.NetManager.PollEvents();


        protected abstract void RegisterHandlers();
    }
}
