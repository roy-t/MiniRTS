using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.GameLogic.BluePrints
{
    public sealed class RCSBluePrint
    {
        public RCSBluePrint(Model model, ExhaustPoint[] exhausts)
        {
            this.Model = model;
            this.Exhausts = exhausts;
        }

        public Model Model { get; }
        public ExhaustPoint[] Exhausts { get; }
    }
}
