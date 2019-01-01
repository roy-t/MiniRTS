namespace MiniEngine.Units
{
    public struct MetersPerSecond
    {
        public MetersPerSecond(float value)
        {
            this.Value = value;
        }

        public float Value { get; set; }

        public static implicit operator MetersPerSecond(float value) => new MetersPerSecond(value);

        public static implicit operator float(MetersPerSecond mps) => mps.Value;

        public static Meters operator *(MetersPerSecond mps, Seconds seconds) => new Meters(mps.Value * seconds.Value);
    }
}
