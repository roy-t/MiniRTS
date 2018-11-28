using System;
using System.Globalization;

namespace MiniEngine.Units
{
    public struct Seconds : IEquatable<Seconds>
    {
        public Seconds(float value)
        {
            this.Value = value;
        }

        public float Value { get; }

        public static implicit operator Seconds(float value) => new Seconds(value);

        public static implicit operator float(Seconds seconds) => seconds.Value;

        public static implicit operator Seconds(TimeSpan value) => new Seconds((float)value.TotalSeconds);

        public static Seconds operator +(Seconds a, Seconds b) => new Seconds(a.Value + b.Value);

        public static Seconds operator -(Seconds a, Seconds b) => new Seconds(a.Value - b.Value);

        public static bool operator >(Seconds a, Seconds b) => a.Value > b.Value;

        public static bool operator <(Seconds a, Seconds b) => a.Value < b.Value;

        public static bool operator >=(Seconds a, Seconds b) => a.Value >= b.Value;

        public static bool operator <=(Seconds a, Seconds b) => a.Value <= b.Value;

        public static bool operator ==(Seconds a, Seconds b) => a.Equals(b);

        public static bool operator !=(Seconds a, Seconds b) => !a.Equals(b);

        public override int GetHashCode() => this.Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Seconds)
            {
                return this.Equals((Seconds)obj);
            }

            return false;
        }

        public bool Equals(Seconds other) => other.Value == this.Value;

        public override string ToString() => $"{this.Value.ToString("F2", CultureInfo.InvariantCulture)}s";
    }   
}
