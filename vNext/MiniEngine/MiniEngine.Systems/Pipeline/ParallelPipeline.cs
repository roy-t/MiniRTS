using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MiniEngine.Systems.Pipeline
{
    public sealed class ParallelPipeline
    {
        private readonly int MaxConcurrency;
        private readonly int ExternalThreadIndex;
        private readonly Thread[] Threads;
        private readonly StageLockPrimitive StartFramePrimitive;
        private readonly StageLockPrimitive EndFramePrimitive;
        private readonly StageLockPrimitive StageStartPrimitive;
        private readonly StageLockPrimitive StageEndPrimitive;

        private PipelineState pipelineState;

        public ParallelPipeline(IReadOnlyList<PipelineStage> pipelineStages)
        {
            this.PipelineStages = pipelineStages;
            this.MaxConcurrency = this.PipelineStages.Max(stage => stage.Systems.Count);
            this.ExternalThreadIndex = this.MaxConcurrency;

            this.StartFramePrimitive = new StageLockPrimitive(this.MaxConcurrency + 1);
            this.EndFramePrimitive = new StageLockPrimitive(this.MaxConcurrency + 1);

            this.StageStartPrimitive = new StageLockPrimitive(this.MaxConcurrency);
            this.StageEndPrimitive = new StageLockPrimitive(this.MaxConcurrency);

            this.Threads = new Thread[this.MaxConcurrency];
            for (var i = 0; i < this.MaxConcurrency; i++)
            {
                this.Threads[i] = new Thread(threadIndex => this.ThreadStart(threadIndex!));
                this.Threads[i].Start(i);
            }

            this.pipelineState = PipelineState.ReadyForNextRun;
        }

        public IReadOnlyList<PipelineStage> PipelineStages { get; }

        public int ActiveThreads => this.Threads.Sum(x => x.IsAlive ? 1 : 0);

        public void Run()
        {
            if (this.pipelineState == PipelineState.ReadyForNextRun)
            {
                this.StartFramePrimitive.DecrementAndWait(this.ExternalThreadIndex);
                this.pipelineState = PipelineState.Running;
            }
            else
            {
                throw new InvalidOperationException($"Cannot call {nameof(Run)} while the {nameof(ParallelPipeline)} is in the {this.pipelineState} state");
            }
        }

        public void Wait()
        {
            if (this.pipelineState == PipelineState.Running)
            {
                this.EndFramePrimitive.DecrementAndWait(this.ExternalThreadIndex);
                this.pipelineState = PipelineState.ReadyForNextRun;
            }
            else
            {
                throw new InvalidOperationException($"Cannot call {nameof(Wait)} while the {nameof(ParallelPipeline)} is in the {this.pipelineState} state");
            }
        }

        public void Stop()
        {
            if (this.pipelineState == PipelineState.Running)
            {
                this.pipelineState = PipelineState.Stopped;
                this.EndFramePrimitive.DecrementAndWait(this.ExternalThreadIndex);

                this.JoinWorkers();
            }
            else if (this.pipelineState == PipelineState.ReadyForNextRun)
            {
                this.pipelineState = PipelineState.Stopped;
                this.StartFramePrimitive.DecrementAndWait(this.ExternalThreadIndex);

                this.JoinWorkers();
            }
            else if (this.pipelineState == PipelineState.Stopped)
            {
                return;
            }
            else
            {
                throw new InvalidOperationException($"Cannot call {nameof(Stop)} while the {nameof(ParallelPipeline)} is in the {this.pipelineState} state");
            }
        }

        private void ThreadStart(object threadIndexObj)
        {
            var threadIndex = (int)threadIndexObj;

            while (this.pipelineState != PipelineState.Stopped)
            {
                this.StartFramePrimitive.DecrementAndWait(threadIndex);
                if (this.pipelineState == PipelineState.Stopped) { return; }

                for (var currentStage = 0; currentStage < this.PipelineStages.Count; currentStage++)
                {
                    this.StageStartPrimitive.DecrementAndWait(threadIndex);

                    var stage = this.PipelineStages[currentStage];
                    if (threadIndex < stage.Systems.Count)
                    {
                        var system = stage.Systems[threadIndex];
                        system.Process();
                    }

                    this.StageEndPrimitive.DecrementAndWait(threadIndex);
                }

                this.EndFramePrimitive.DecrementAndWait(threadIndex);
            }
        }

        private void JoinWorkers()
        {
            for (var i = 0; i < this.Threads.Length; i++)
            {
                this.Threads[i].Join();
            }
        }
    }
}
