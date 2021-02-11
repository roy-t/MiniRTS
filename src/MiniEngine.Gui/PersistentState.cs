using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

namespace MiniEngine.Gui
{
    public sealed class PersistentState<T>
    {
        private readonly ILogger Logger;
        private readonly string Filename;
        private readonly JsonSerializerOptions Options;

        public PersistentState(ILogger logger, string filename)
        {
            this.Logger = logger;
            this.Filename = filename;
            this.Options = new JsonSerializerOptions
            {
                IncludeFields = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
        }

        public void Reset()
        {
            try
            {
                File.Delete(this.Filename);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to remove {@file}", this.Filename);
            }
        }

        public void Serialize(T target)
        {
            try
            {
                var json = JsonSerializer.Serialize(target, this.Options);
                File.WriteAllText(this.Filename, json);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to serialize {@file}", Filename);
            }
        }

        public T? Deserialize()
        {
            try
            {
                var json = File.ReadAllText(Filename);
                var values = JsonSerializer.Deserialize<T>(json, this.Options);
                return values;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to deserialize {@file}", Filename);
            }

            return default;
        }
    }
}
