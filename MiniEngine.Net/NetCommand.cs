using Microsoft.Xna.Framework;

namespace MiniEngine.Net
{
    public class NetCommand
    {
        public static NetCommand Create(Vector3 where, int commandId, string payload)
        {
            return new NetCommand()
            {
                Where = where,
                CommandId = commandId,
                Payload = payload
            };
        }

        public Vector3 Where { get; set; }

        public int CommandId { get; set; }
        public string Payload { get; set; }
    }
}
