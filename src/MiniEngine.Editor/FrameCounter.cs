using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.Editor
{
    internal sealed class FrameCounter
    {
        private TimeSpan timer;
        private int accumulator;

        public FrameCounter()
        {
            this.timer = TimeSpan.Zero;
            this.accumulator = 0;

            this.MillisecondsPerFrame = 1.0 / 60.0;
            this.FramesPerSecond = 60;
        }

        public bool Update(GameTime gameTime)
        {
            this.timer += gameTime.ElapsedGameTime;
            if (this.timer < TimeSpan.FromSeconds(1))
            {
                ++this.accumulator;
                return false;
            }
            else
            {
                this.MillisecondsPerFrame = this.timer.TotalMilliseconds / this.accumulator;
                this.FramesPerSecond = this.accumulator;

                this.timer = TimeSpan.Zero;
                this.accumulator = 0;
                return true;
            }
        }

        public double MillisecondsPerFrame { get; private set; }
        public int FramesPerSecond { get; private set; }
    }
}
