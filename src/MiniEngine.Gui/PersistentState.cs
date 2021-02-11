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

        public PersistentState(ILogger logger, params JsonConverter[] converters)
            : this(logger, "", converters) { }

        public PersistentState(ILogger logger, string filename, params JsonConverter[] converters)
        {
            this.Logger = logger;
            this.Filename = filename;
            this.Options = new JsonSerializerOptions
            {
                IncludeFields = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };

            foreach (var converter in converters)
            {
                this.Options.Converters.Add(converter);
            }
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

        public string Serialize(T target) => JsonSerializer.Serialize(target, this.Options);

        public void Save(T target)
        {
            try
            {
                var json = this.Serialize(target);
                File.WriteAllText(this.Filename, json);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to serialize {@file}", this.Filename);
            }
        }

        public T? Deserialize(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, this.Options);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to deserialize {@json}", json);
                return default;
            }
        }


        public T? Load()
        {
            try
            {
                var json = File.ReadAllText(this.Filename);
                var values = this.Deserialize(json);
                return values;
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to read {@file}", this.Filename);
            }

            return default;
        }
    }
}
