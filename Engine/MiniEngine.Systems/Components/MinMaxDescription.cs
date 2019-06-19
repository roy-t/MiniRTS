using Microsoft.Xna.Framework;

namespace MiniEngine.Systems.Components
{
    public class MinMaxDescription
    {
        private MinMaxDescription(MinMaxDescriptionType type, float min, float max)
        {
            this.Type = type;
            this.Min = min;
            this.Max = max;
        }

        public MinMaxDescription(float min, float max)
            : this(MinMaxDescriptionType.Custom, min, max)
        {

        }

        public MinMaxDescriptionType Type { get; }
        public float Min { get; }
        public float Max { get; }


        public static MinMaxDescription None => new MinMaxDescription(MinMaxDescriptionType.None, 0, 0);
        public static MinMaxDescription ZeroToOne => new MinMaxDescription(MinMaxDescriptionType.ZeroToOne, 0, 1);
        public static MinMaxDescription MinusOneToOne => new MinMaxDescription(MinMaxDescriptionType.MinusOneToOne, -1, 1);
        public static MinMaxDescription MinusInfinityToInfinity => new MinMaxDescription(MinMaxDescriptionType.MinusInfinityToInfinity, float.NegativeInfinity, float.PositiveInfinity);
        public static MinMaxDescription ZeroToInfinity => new MinMaxDescription(MinMaxDescriptionType.ZeroToInfinity, 0, float.PositiveInfinity);

        public static MinMaxDescription MinusPiToPi => new MinMaxDescription(-MathHelper.Pi, MathHelper.Pi);
    }

    public enum MinMaxDescriptionType
    {
        None,
        ZeroToOne,
        MinusOneToOne,
        MinusInfinityToInfinity,
        ZeroToInfinity,
        Custom
    }
}
