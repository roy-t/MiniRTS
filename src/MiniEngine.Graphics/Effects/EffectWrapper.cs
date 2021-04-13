using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using Serilog;

namespace MiniEngine.Graphics.Effects
{
    [Content]
    public abstract class EffectWrapper : IDisposable
    {
        private FileSystemWatcher? watcher;
        private readonly ILogger Logger;

        public EffectWrapper(ILogger logger, Effect effect)
        {
            this.Logger = logger;
            this.Effect = effect;

            if (this.Effect.Tag is string path)
            {
                this.CreateHotReloader(path);
            }
        }

        protected Effect Effect { get; private set; }

        protected abstract void Reload();

        [Conditional("DEBUG")]
        private void CreateHotReloader(string path)
        {
            if (File.Exists(path))
            {
                var directory = Path.GetDirectoryName(path) ?? string.Empty;
                var file = Path.GetFileName(path);
                this.watcher = new FileSystemWatcher(directory)
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

                this.watcher.Created += (s, e) => this.RebuildAndReload(path, "created");
                this.watcher.Deleted += (s, e) => this.RebuildAndReload(path, "deleted");
                this.watcher.Renamed += (s, e) => this.RebuildAndReload(path, "renamed");
                this.watcher.Changed += (s, e) => this.RebuildAndReload(path, "changed");

                this.watcher.Error += (s, e) => this.Logger.Error(e.GetException(), "FileSystemWatcher failed watching {@file}", path);
            }
        }

        private void RebuildAndReload(string path, string reason)
        {
            try
            {
                var builder = new EffectBuilder();
                this.Effect = builder.Build(this.Effect.GraphicsDevice, path);
                this.Reload();
                this.Logger.Information("Reloaded {@reason} - {@file}", reason, path);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to reload {@reason} - {@file}", reason, path);
            }
        }

        public void Dispose()
            => this.watcher?.Dispose();
    }
}
