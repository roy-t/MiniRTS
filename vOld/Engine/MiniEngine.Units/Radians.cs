using System;
using System.Globalization;

namespace MiniEngine.Units
{
    public struct Radians : IEquatable<Radians>
    {
        public Radians(float value)
        {
            this.Value = value;
        }

        public float Value { get; }

        public static implicit operator Radians(float value) => new Radians(value);

        public static implicit operator float(Radians radians) => radians.Value;

        public static Radians operator +(Radians a, Radians b) => new Radians(a.Value + b.Value);

        public static Radians operator -(Radians a, Radians b) => new Radians(a.Value - b.Value);

        public static bool operator >(Radians a, Radians b) => a.Value > b.Value;

        public static bool operator <(Radians a, Radians b) => a.Value < b.Value;

        public static bool operator >=(Radians a, Radians b) => a.Value >= b.Value;

        public static bool operator <=(Radians a, Radians b) => a.Value <= b.Value;

        public static bool operator ==(Radians a, Radians b) => a.Equals(b);

        public static bool operator !=(Radians a, Radians b) => !a.Equals(b);

        public override int GetHashCode() => this.Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Radians)
            {
                return this.Equals((Radians)obj);
            }

            return false;
        }

        public bool Equals(Radians other) => other.Value == this.Value;

        public override string ToString() => $"{this.Value.ToString("F2", CultureInfo.InvariantCulture)}s";

        public static RadianPerSecond operator /(Radians radians, Seconds seconds) => new RadianPerSecond(radians.Value / seconds.Value);

        public static Radians Pi => new Radians(3.14159274f);
    }
}
