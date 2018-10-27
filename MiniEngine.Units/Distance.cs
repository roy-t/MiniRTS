namespace MiniEngine.Units
{
    public struct Meters
    {
        public Meters(float value)
        {
            this.Value = value;
        }

        public float Value { get; set; }

        public static implicit operator Meters(float value)
        {
            return new Meters(value);
        }

        public static implicit operator float(Meters meters)
        {
            return meters.Value;
        }
    }
}
