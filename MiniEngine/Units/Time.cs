using System;

namespace MiniEngine.Units
{
    public struct Seconds
    {       
        public Seconds(float value)
        {
            this.Value = value;
        }

        public float Value { get; set; }

        public static implicit operator Seconds(float value)
        {
            return new Seconds(value);
        }

        public static implicit operator Seconds(TimeSpan value)
        {
            return new Seconds((float)value.TotalSeconds);
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
