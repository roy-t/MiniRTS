namespace MiniEngine.Units
{
    public struct RadianPerSecond
    {
        public RadianPerSecond(float value)
        {
            this.Value = value;
        }

        public float Value { get; set; }

        public static implicit operator RadianPerSecond(float value) => new RadianPerSecond(value);

        public static implicit operator float(RadianPerSecond radians) => radians.Value;

        public static Radians operator *(RadianPerSecond rps, Seconds seconds) => new Radians(rps.Value * seconds.Value);
    }
}
