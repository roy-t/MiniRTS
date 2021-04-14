using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace MiniEngine.Graphics.Effects
{
    public sealed class EffectReloader : IDisposable
    {
        private readonly ILogger Logger;
        private readonly string Asset;
        private readonly GraphicsDevice Device;
        private readonly FileSystemWatcher Watcher;

        private readonly Subject<Effect> EffectSubject;

        private readonly Subject<Unit> ReloadSubject;
        private readonly IDisposable Subscription;

        public EffectReloader(ILogger logger, string asset, GraphicsDevice device)
        {
            this.Logger = logger;
            this.Asset = asset;
            this.Device = device;
            this.Watcher = CreateWatcher(asset);

            this.Watcher.Created += (s, e) => this.OnChange(asset, "Created");
            this.Watcher.Deleted += (s, e) => this.OnChange(asset, "Deleted");
            this.Watcher.Renamed += (s, e) => this.OnChange(asset, "Renamed");
            this.Watcher.Changed += (s, e) => this.OnChange(asset, "Changed");

            this.Watcher.Error += (s, e) => this.Logger.Error(e.GetException(), "FileSystemWatcher failed watching {@file}", asset);

            this.EffectSubject = new Subject<Effect>();

            this.ReloadSubject = new Subject<Unit>();
            this.Subscription = this.ReloadSubject
                .Throttle(TimeSpan.FromMilliseconds(15))
                .Do(_ => this.Reload())
                .Subscribe();
        }

        public IObservable<Effect> OnEffectReloaded
            => this.EffectSubject.AsObservable();

        private void OnChange(string path, string reason)
        {
            this.Logger.Debug("[{@reason}] {@file}", reason, path);
            this.ReloadSubject.OnNext(Unit.Default);
        }

        private void Reload()
        {
            var builder = new EffectBuilder(this.Logger);
            var effect = builder.Build(this.Device, this.Asset);
            if (effect != null)
            {
                this.EffectSubject.OnNext(effect);
                this.Logger.Information("Reloaded {@file}", this.Asset);
            }
        }

        private static FileSystemWatcher CreateWatcher(string asset)
        {
            var directory = Path.GetDirectoryName(asset) ?? string.Empty;
            var file = Path.GetFileName(asset);
            return new FileSystemWatcher(directory)
            {
                NotifyFilter = NotifyFilters.Attributes
                            | NotifyFilters.CreationTime
                            | NotifyFilters.DirectoryName
                            | NotifyFilters.FileName
                            | NotifyFilters.LastAccess
                            | NotifyFilters.LastWrite
                            | NotifyFilters.Security
                            | NotifyFilters.Size,
                Filter = file,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true,
            };
        }

        public void Dispose()
        {
            this.Watcher.Dispose();
            this.Subscription.Dispose();
        }
    }
}
