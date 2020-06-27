using LiteNetLib.Utils;

namespace MiniEngine.Net
{
    public struct Player : INetSerializable
    {
        public void Deserialize(NetDataReader reader) => this.Id = reader.GetByte();
        public void Serialize(NetDataWriter writer) => writer.Put(this.Id);

        public byte Id { get; set; }

        public override string ToString() => $"Player {this.Id}";
    }
}
