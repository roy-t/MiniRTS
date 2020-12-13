using System;
using System.Globalization;

namespace MiniEngine.Units
{
    public struct Kilograms : IEquatable<Kilograms>
    {
        public Kilograms(float value)
        {
            this.Value = value;
        }

        public float Value { get; }

        public static implicit operator Kilograms(float value) => new Kilograms(value);

        public static implicit operator float(Kilograms kilograms) => kilograms.Value;

        public static Kilograms operator +(Kilograms a, Kilograms b) => new Kilograms(a.Value + b.Value);

        public static Kilograms operator -(Kilograms a, Kilograms b) => new Kilograms(a.Value - b.Value);

        public static bool operator >(Kilograms a, Kilograms b) => a.Value > b.Value;

        public static bool operator <(Kilograms a, Kilograms b) => a.Value < b.Value;

        public static bool operator >=(Kilograms a, Kilograms b) => a.Value >= b.Value;

        public static bool operator <=(Kilograms a, Kilograms b) => a.Value <= b.Value;

        public static bool operator ==(Kilograms a, Kilograms b) => a.Equals(b);

        public static bool operator !=(Kilograms a, Kilograms b) => !a.Equals(b);

        public override int GetHashCode() => this.Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Kilograms)
            {
                return this.Equals((Kilograms)obj);
            }

            return false;
        }

        public bool Equals(Kilograms other) => other.Value == this.Value;

        public override string ToString() => $"{this.Value.ToString("F2", CultureInfo.InvariantCulture)}s";
    }
}
