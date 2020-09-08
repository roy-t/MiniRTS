using System.Threading;

namespace MiniEngine.Pipeline.Next
{
    public sealed class ThreadingPrimitive
    {
        private readonly int MaxCount;
        private readonly int Sleep1Threshold;
        private readonly bool[] Flags;
        private readonly SpinWait[] Spins;

        public ThreadingPrimitive(int maxCount, int sleep1Threshold = -1)
        {
            this.MaxCount = maxCount;
            this.Sleep1Threshold = sleep1Threshold;
            this.Flags = new bool[maxCount];
            this.Spins = new SpinWait[maxCount];

            this.Reset();
        }

        public void DecrementAndWait(int threadIndex)
        {
            this.Flags[threadIndex] = false;

            if (threadIndex == 0)
            {
                while (this.CountSum() > 0)
                {
                    this.Spins[threadIndex].SpinOnce(this.Sleep1Threshold);
                }
                this.Reset();
            }

            while (this.CountSum() < this.MaxCount)
            {
                this.Spins[threadIndex].SpinOnce(this.Sleep1Threshold);
            }

            this.Spins[threadIndex].Reset();
        }

        private void Reset()
        {
            for (var i = 0; i < this.MaxCount; i++)
            {
                this.Flags[i] = true;
            }
        }

        private int CountSum()
        {
            var sum = 0;
            for (var i = 0; i < this.MaxCount; i++)
            {
                sum += this.Flags[i] ? 1 : 0;
            }

            return sum;
        }
    }
}

