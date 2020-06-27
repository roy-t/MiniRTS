namespace MiniEngine.Net
{
    public class NetCommand
    {
        public static NetCommand Create(byte playerId, int commandId, string payload)
        {
            return new NetCommand()
            {
                Player = new Player() { Id = playerId },
                CommandId = commandId,
                Payload = payload
            };
        }

        public Player Player { get; set; }

        public int CommandId { get; set; }
        public string Payload { get; set; }
    }
}
