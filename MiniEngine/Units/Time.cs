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

        public static implicit operator Seconds(float value)
        {
            return new Seconds(value);
        }

        public static implicit operator Seconds(TimeSpan value)
        {
            return new Seconds((float)value.TotalSeconds);
        }

        public static Seconds operator +(Seconds a, Seconds b)
        {
            return new Seconds(a.Value + b.Value);
        }

        public static Seconds operator -(Seconds a, Seconds b)
        {
            return new Seconds(a.Value - b.Value);
        }

        public static bool operator >(Seconds a, Seconds b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(Seconds a, Seconds b)
        {
            return a.Value < b.Value;
        }

        public static bool operator >=(Seconds a, Seconds b)
        {
            return a.Value >= b.Value;
        }

        public static bool operator <=(Seconds a, Seconds b)
        {
            return a.Value <= b.Value;
        }

        public static bool operator ==(Seconds a, Seconds b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Seconds a, Seconds b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Seconds)
            {
                return Equals((Seconds) obj);
            }

            return false;
        }
        
        public bool Equals(Seconds other)
        {
            return other.Value == this.Value;
        }

        public override string ToString()
        {
            return $"{this.Value.ToString(CultureInfo.InvariantCulture)}s";
        }
    }

    public struct Minutes
    {
        public Minutes(float value)
        {
            this.Value = value;
        }

        public float Value { get; set; }

        public static implicit operator Minutes(Seconds seconds)
        {
            return new Minutes(seconds.Value / 60.0f);
        }
    }


    public struct Hours
    {
        public Hours(float value)
        {
            this.Value = value;
        }

        public float Value { get; set; }

        public static implicit operator Hours(Minutes minutes)
        {
            return new Hours(minutes.Value / 60.0f);
        }
    }
}
