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
        private readonly ILogger Logger;
        private EffectReloader? reloader;
        private IDisposable? subscription;

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
                this.reloader = new EffectReloader(this.Logger, path, this.Effect.GraphicsDevice);
                this.subscription = this.reloader.OnEffectReloaded.Subscribe(effect =>
                {
                    this.Effect = effect;
                    this.Reload();
                });
            }
            else
            {
                this.Logger.Warning("Could not create reloader for {@file}, file not found", path);
            }
        }

        public void Dispose()
        {
            this.reloader?.Dispose();
            this.subscription?.Dispose();
        }
    }
}
