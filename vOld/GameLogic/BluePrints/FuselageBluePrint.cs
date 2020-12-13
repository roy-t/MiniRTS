using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.GameLogic.BluePrints
{
    public sealed class FuselageBluePrint
    {
        public FuselageBluePrint(Model model, ConnectorType topConnector = ConnectorType.Medium, ConnectorType bottomConnector = ConnectorType.Medium, float height = 4.0f, float radius = 1.0f, bool allowAddons = true, params ExhaustPoint[] exhausts)
        {
            this.TopConnector = topConnector;
            this.BottomConnector = bottomConnector;
            this.Height = height;
            this.Radius = radius;
            this.Model = model;
            this.AllowAddons = allowAddons;
            this.Exhausts = exhausts ?? new ExhaustPoint[0];
        }

        public ConnectorType TopConnector { get; }
        public ConnectorType BottomConnector { get; }

        public float Height { get; }

        public float Radius { get; }

        public Model Model { get; }

        public bool AllowAddons { get; }

        public ExhaustPoint[] Exhausts { get; }
    }


    public enum ConnectorType : byte
    {
        None,
        Medium,
    }
}
