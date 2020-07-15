using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.GameLogic.BluePrints
{
    public sealed class RCSBluePrint
    {
        public RCSBluePrint(Model model, ExhaustBluePrint[] exhaustOffsets)
        {
            this.Model = model;
            this.ExhaustOffsets = exhaustOffsets;
        }

        public Model Model { get; }
        public ExhaustBluePrint[] ExhaustOffsets { get; }
    }
}
