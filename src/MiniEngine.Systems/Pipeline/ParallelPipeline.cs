using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MiniEngine.Systems.Pipeline
{
    public sealed class ParallelPipeline : IDisposable
    {
        private readonly ConcurrentQueue<ISystemBinding> Work;
        private readonly Thread[] Threads;

        private readonly ManualResetEventSlim StageStartEvent;
        private readonly CountdownEvent WorkCountdownEvent;
        private readonly CancellationTokenSource CancellationToken;

        public ParallelPipeline(IReadOnlyList<PipelineStage> stages)
        {
            this.Stages = stages;
            this.Work = new ConcurrentQueue<ISystemBinding>();

            this.StageStartEvent = new ManualResetEventSlim(false);
            this.WorkCountdownEvent = new CountdownEvent(0);
            this.CancellationToken = new CancellationTokenSource();

            var maxThreads = stages.Max(stage => stage.Systems.Count);
            this.Threads = new Thread[maxThreads];
            for (var i = 0; i < this.Threads.Length; i++)
            {
                this.Threads[i] = new Thread(this.Process)
                {
                    Name = $"PipelineThread {i}",
                    IsBackground = true
                };

                this.Threads[i].Start();
            }
        }

        public IReadOnlyList<PipelineStage> Stages { get; }

        public void Frame()
        {
            try
            {
                this.WorkCountdownEvent.Wait(this.CancellationToken.Token);

                for (var i = 0; i < this.Stages.Count; i++)
                {
                    var stage = this.Stages[i];
                    if (stage.Systems.Count == 1)
                    {
                        stage.Systems[0].Process();
                    }
                    else
                    {
                        for (var j = 0; j < stage.Systems.Count; j++)
                        {
                            this.Work.Enqueue(stage.Systems[j]);
                        }

                        this.WorkCountdownEvent.Reset(stage.Systems.Count);

                        this.StageStartEvent.Set();

                        this.WorkCountdownEvent.Wait(this.CancellationToken.Token);

                        this.StageStartEvent.Reset();
                    }
                }
            }
            catch (OperationCanceledException) { }
        }

        public void Dispose()
        {
            this.CancellationToken.Cancel();
            for (var i = 0; i < this.Threads.Length; i++)
            {
                this.Threads[i].Join();
            }

            this.CancellationToken.Dispose();
            this.StageStartEvent.Dispose();
            this.WorkCountdownEvent.Dispose();
        }

        private void Process()
        {
            try
            {
                while (true)
                {
                    this.StageStartEvent.Wait(this.CancellationToken.Token);

                    if (this.Work.TryDequeue(out var system))
                    {
                        system.Process();
                        this.WorkCountdownEvent.Signal();
                    }
                }

            }
            catch (OperationCanceledException) { }
        }
    }
}
