using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using Serilog;

namespace MiniEngine.Gui
{
    public enum ToolType { Slider }
    public sealed class Tool
    {
        public Tool(string Property, ToolType Type, float Min, float Max)
        {
            this.Property = Property;
            this.Type = Type;
            this.Min = Min;
            this.Max = Max;
        }

        public readonly string Property;
        public ToolType Type;
        public float Min;
        public float Max;
    }

    [Service]
    public sealed class EditorState : IDisposable
    {
        private static readonly string Filename = "NewEditorState.json";
        private readonly JsonSerializerOptions Options;
        private readonly ILogger Logger;

        private readonly Trie Properties;
        private readonly Dictionary<string, Tool> Tools;

        public EditorState(ILogger logger)
        {
            this.Logger = logger;
            this.Properties = new Trie();
            this.Tools = new Dictionary<string, Tool>();
            this.Options = new JsonSerializerOptions();
            this.Deserialize();
        }

        public void Dispose()
            => this.Serialize();

        float foo;
        float bar;
        float mi;
        float ma;
        float min = 0.0f;
        float max = 1.0f;
        string path = "";
        public void Update()
        {
            if (ImGui.Begin("Window1"))
            {
                // TOO
                var id = new IntPtr(1);
                ImGui.PushID(id);
                ImGui.SliderFloat("Label1", ref foo, min, max);
                ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Button, Color.Black.ToVector4());
                if (ImGui.SmallButton("\u00BB"))
                {
                    path = "some.path";
                    mi = min;
                    ma = max;
                }
                ImGui.PopStyleColor();
                if (path != string.Empty)
                {
                    ImGui.OpenPopup("Tool Picker");
                    if (ImGui.BeginPopup("Tool Picker"))
                    {
                        ImGui.InputFloat("Min", ref mi);
                        ImGui.InputFloat("Max", ref ma);
                        if (ImGui.Button("Save"))
                        {
                            min = mi;
                            max = ma;
                            path = string.Empty;
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("Cancel"))
                        {
                            path = string.Empty;
                        }

                        ImGui.EndPopup();
                    }
                }

                ImGui.PopID();

                ImGui.PushID("B");
                ImGui.SliderFloat("Label1", ref bar, 0, 1);
                ImGui.PopID();

                ImGui.End();
            }

            if (ImGui.Begin("Window2"))
            {

                ImGui.SliderFloat("Label1", ref bar, 0, 1);

                ImGui.End();
            }

        }

        public void StoreRange(Tool range)
        {
            var sortKey = CreateSortKey(range.Property);
            this.Properties.Add(sortKey);
            this.Tools.Add(sortKey, range);
        }

        public Tool GetRange(string propertyPath)
        {
            var sortKey = CreateSortKey(propertyPath);
            var bestMatch = this.Properties.FindTextWithLongestCommonPrefix(sortKey);
            if (bestMatch != string.Empty)
            {
                return this.Tools[bestMatch];
            }

            return new Tool(propertyPath, ToolType.Slider, float.MinValue, float.MaxValue);
        }
        private void Serialize()
        {
            try
            {
                var ranges = this.Tools.Values.ToList();
                var json = JsonSerializer.Serialize(ranges, this.Options);
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
                var ranges = JsonSerializer.Deserialize<List<Tool>>(json, this.Options)
                    ?? new List<Tool>();

                foreach (var range in ranges)
                {
                    this.StoreRange(range);
                }
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
