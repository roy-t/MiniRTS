using System;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Configuration;
using Serilog;

namespace MiniEngine.Gui.Tools
{
    [Service]
    public sealed class ToolLinker : IDisposable
    {
        private record ToolPair(string Key, ToolState Value);

        private readonly PersistentState<List<ToolPair>> State;
        private readonly Trie KnownTools;
        private readonly Dictionary<string, ToolState> Tools;

        public ToolLinker(ILogger logger)
        {
            this.State = new PersistentState<List<ToolPair>>(logger, "Tools.json");
            this.KnownTools = new Trie();
            this.Tools = new Dictionary<string, ToolState>();
            this.Deserialize();
        }

        public void Reset()
        {
            this.State.Reset();
            this.KnownTools.Clear();
            this.Tools.Clear();
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
            var values = this.Tools.Select(kv => new ToolPair(kv.Key, kv.Value)).ToList();
            this.State.Serialize(values);
        }

        private void Deserialize()
        {
            var values = this.State.Deserialize() ?? new List<ToolPair>();
            foreach (var tuple in values)
            {
                this.Link(tuple.Key, tuple.Value);
            }
        }

        public void Dispose() => this.Serialize();
    }
}
