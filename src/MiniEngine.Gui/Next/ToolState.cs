using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using MiniEngine.Configuration;
using Serilog;

namespace MiniEngine.Gui.Next
{
    [Service]
    public sealed class ToolState : IDisposable
    {
        private record ToolPair(string Key, Tool Value);

        private readonly ILogger Logger;
        private readonly Trie KnownTools;
        private readonly Dictionary<string, Tool> Tools;
        private readonly JsonSerializerOptions Options;
        private static readonly string Filename = "Tools.json";

        public ToolState(ILogger logger)
        {
            this.Logger = logger;
            this.KnownTools = new Trie();
            this.Tools = new Dictionary<string, Tool>();
            this.Options = new JsonSerializerOptions
            {
                IncludeFields = true
            };

            this.Deserialize();
        }

        public void Reset()
        {
            try
            {
                File.Delete(Filename);
                this.KnownTools.Clear();
                this.Tools.Clear();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to remove {@file}", Filename);
            }
        }

        public void Set(Property property, Tool tool)
            => this.Set(property.SortKey, tool);

        public Tool Get(Property property)
        {
            var bestMatch = this.KnownTools.FindTextWithLongestCommonPrefix(property.SortKey);
            if (bestMatch != property.SortKey)
            {
                if (bestMatch != string.Empty)
                {
                    this.Set(property, this.Tools[bestMatch]);
                }
                else
                {
                    this.Set(property, new Tool(string.Empty, 0.0f, 1.0f, 0.0f));
                }
            }

            return this.Tools[property.SortKey];
        }

        private void Set(string sortKey, Tool tool)
        {
            this.KnownTools.Add(sortKey);
            this.Tools[sortKey] = tool;
        }

        private void Serialize()
        {
            try
            {
                var values = this.Tools.Select(kv => new ToolPair(kv.Key, kv.Value)).ToList();
                var json = JsonSerializer.Serialize(values, this.Options);
                File.WriteAllText(Filename, json);
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to serialize {@file}", Filename);
            }
        }

        private void Deserialize()
        {
            try
            {
                var json = File.ReadAllText(Filename);
                var values = JsonSerializer.Deserialize<List<ToolPair>>(json, this.Options)
                    ?? new List<ToolPair>();

                foreach (var tuple in values)
                {
                    this.Set(tuple.Key, tuple.Value);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to deserialize {@file}", Filename);
            }
        }

        public void Dispose() => this.Serialize();
    }
}
