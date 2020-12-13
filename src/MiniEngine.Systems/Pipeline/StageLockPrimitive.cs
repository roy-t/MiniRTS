using System.Threading;

namespace MiniEngine.Systems.Pipeline
{
    public sealed class StageLockPrimitive
    {
        private const int Sleep1Threshold = -1;
        private readonly int MaxCount;
        private readonly SpinWait[] Spins;
        private int counter;

        public StageLockPrimitive(int maxCount)
        {
            this.MaxCount = maxCount;
            this.Spins = new SpinWait[maxCount];

            this.Reset();
        }

        public void DecrementAndWait(int threadIndex)
        {
            Interlocked.Increment(ref this.counter);

            if (threadIndex == 0)
            {
                while (this.counter < this.MaxCount)
                {
                    this.Spins[threadIndex].SpinOnce(Sleep1Threshold);
                }

                this.Reset();
                return;
            }

            while (this.counter != 0)
            {
                this.Spins[threadIndex].SpinOnce(Sleep1Threshold);
            }
        }

        private void Reset() => this.counter = 0;
    }
}

