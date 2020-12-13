using System;
using System.Globalization;

namespace MiniEngine.Units
{
    public struct Meters : IEquatable<Meters>
    {
        public Meters(float value)
        {
            this.Value = value;
        }

        public float Value { get; }

        public static implicit operator Meters(float value) => new Meters(value);

        public static implicit operator float(Meters meters) => meters.Value;

        public static Meters operator +(Meters a, Meters b) => new Meters(a.Value + b.Value);

        public static Meters operator -(Meters a, Meters b) => new Meters(a.Value - b.Value);

        public static bool operator >(Meters a, Meters b) => a.Value > b.Value;

        public static bool operator <(Meters a, Meters b) => a.Value < b.Value;

        public static bool operator >=(Meters a, Meters b) => a.Value >= b.Value;

        public static bool operator <=(Meters a, Meters b) => a.Value <= b.Value;

        public static bool operator ==(Meters a, Meters b) => a.Equals(b);

        public static bool operator !=(Meters a, Meters b) => !a.Equals(b);

        public override int GetHashCode() => this.Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Meters)
            {
                return this.Equals((Meters)obj);
            }

            return false;
        }

        public bool Equals(Meters other) => other.Value == this.Value;

        public override string ToString() => $"{this.Value.ToString("F2", CultureInfo.InvariantCulture)}s";

        public static MetersPerSecond operator /(Meters meters, Seconds seconds) => new MetersPerSecond(meters.Value / seconds.Value);
    }
}
