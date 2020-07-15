using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.GameLogic.BluePrints
{
    public sealed class RocketFuselageBluePrint
    {
        public RocketFuselageBluePrint(Model model, ConnectorType topConnector = ConnectorType.Medium, ConnectorType bottomConnector = ConnectorType.Medium, float height = 4.0f, float radius = 1.0f, bool allowRCS = true, params ExhaustBluePrint[] exhaustOffsets)
        {
            this.TopConnector = topConnector;
            this.BottomConnector = bottomConnector;
            this.Height = height;
            this.Radius = radius;
            this.Model = model;
            this.AllowRCS = allowRCS;
            this.ExhaustOffsets = exhaustOffsets ?? new ExhaustBluePrint[0];
        }

        public ConnectorType TopConnector { get; }
        public ConnectorType BottomConnector { get; }

        public float Height { get; }

        public float Radius { get; }

        public Model Model { get; }

        public bool AllowRCS { get; }

        public ExhaustBluePrint[] ExhaustOffsets { get; }
    }


    public enum ConnectorType : byte
    {
        None,
        Medium,
    }
}
