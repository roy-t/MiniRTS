namespace MiniEngine.Units
{
    public struct MetersPerSecond
    {
        public MetersPerSecond(float value)
        {
            this.Value = value;
        }

        public float Value { get; set; }

        public static implicit operator MetersPerSecond(float value)
        {
            return new MetersPerSecond(value);
        }

        public static Meters operator *(MetersPerSecond mps, Seconds seconds)
        {
            return new Meters(mps.Value * seconds.Value);
        }
    }
}
