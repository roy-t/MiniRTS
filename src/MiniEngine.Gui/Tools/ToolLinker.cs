using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using MiniEngine.Configuration;
using Serilog;

namespace MiniEngine.Gui.Tools
{
    [Service]
    public sealed class ToolLinker : IDisposable
    {
        private record ToolPair(string Key, ToolState Value);

        private readonly ILogger Logger;
        private readonly Trie KnownTools;
        private readonly Dictionary<string, ToolState> Tools;
        private readonly JsonSerializerOptions Options;
        private static readonly string Filename = "Tools.json";

        public ToolLinker(ILogger logger)
        {
            this.Logger = logger;
            this.KnownTools = new Trie();
            this.Tools = new Dictionary<string, ToolState>();
            this.Options = new JsonSerializerOptions
            {
                IncludeFields = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
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

        public void Link(Property property, ToolState tool)
            => this.Link(property.SortKey, tool);

        public ToolState Get(Property property)
        {
            var bestMatch = this.KnownTools.FindTextWithLongestCommonPrefix(property.SortKey);
            if (bestMatch != property.SortKey)
            {
                if (bestMatch != string.Empty)
                {
                    this.Link(property, this.Tools[bestMatch]);
                }
                else
                {
                    this.Link(property, new ToolState(string.Empty, 0.0f, 1.0f, 0.0f));
                }
            }

            return this.Tools[property.SortKey];
        }

        private void Link(string sortKey, ToolState tool)
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
                    this.Link(tuple.Key, tuple.Value);
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
