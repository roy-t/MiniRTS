using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MiniEngine.Configuration;
using Serilog;

namespace MiniEngine.Gui
{
    public enum ToolType { Slider }
    public record Range(string Property, ToolType Tool, float Min, float Max);

    [Service]
    public sealed class EditorState : IDisposable
    {
        private static readonly string Filename = "NewEditorState.json";
        private readonly JsonSerializerOptions Options;
        private readonly ILogger Logger;

        private readonly Trie Properties;
        private readonly Dictionary<string, Range> Ranges;

        public EditorState(ILogger logger)
        {
            this.Logger = logger;
            this.Properties = new Trie();
            this.Ranges = new Dictionary<string, Range>();
            this.Options = new JsonSerializerOptions();
            this.Deserialize();
        }

        public void Dispose()
            => this.Deserialize();

        public void StoreRange(Range range)
        {
            var sortKey = CreateSortKey(range.Property);
            this.Properties.Add(sortKey);
            this.Ranges.Add(sortKey, range);
        }

        public Range GetRange(string propertyPath)
        {
            var sortKey = CreateSortKey(propertyPath);
            var bestMatch = this.Properties.FindTextWithLongestCommonPrefix(sortKey);
            if (bestMatch != string.Empty)
            {
                return this.Ranges[bestMatch];
            }

            return new Range(propertyPath, ToolType.Slider, float.MinValue, float.MaxValue);
        }
        private void Serialize()
        {
            try
            {

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

            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to deserialize {@file}", Filename);
            }
        }

        private static string CreateSortKey(string property)
        {
            var path = property.Split('.', StringSplitOptions.RemoveEmptyEntries);
            return string.Join('.', path.Reverse());
        }

    }
}
