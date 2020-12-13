using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Editor.Workspaces.Converters;
using MiniEngine.Systems;
using Serilog;

namespace MiniEngine.Editor.Workspaces
{
    [Service]
    public sealed class EditorStateSerializer
    {
        private static readonly string Filename = "EditorState.json";
        private readonly ILogger Logger;
        private readonly JsonSerializerOptions Options;

        public EditorStateSerializer(ILogger logger)
        {
            this.Logger = logger;

            this.Options = new JsonSerializerOptions();
            this.Options.Converters.Add(new Vector3Converter());
        }

        public EditorState Deserialize()
            => this.Deserialize(Filename, Encoding.UTF8) ?? new EditorState(Entity.Zero, Vector3.Zero, Vector3.Forward, null);

        public void Serialize(EditorState editorState)
            => this.Serialize(editorState, Filename, Encoding.UTF8);

        private void Serialize(EditorState editorState, string filename, Encoding encoding)
        {
            try
            {
                var json = JsonSerializer.Serialize(editorState, this.Options);
                File.WriteAllText(Filename, json, encoding);
            }
            catch (Exception ex)
            {
                this.Logger.Warning(ex, "Failed to write file {@file}", filename);
            }
        }

        private EditorState? Deserialize(string filename, Encoding encoding)
        {
            try
            {
                var json = File.ReadAllText(Filename, encoding);
                return JsonSerializer.Deserialize<EditorState>(json, this.Options);
            }
            catch (Exception ex)
            {
                this.Logger.Warning(ex, "Failed to read file {@file}", filename);
            }

            return null;
        }
    }
}
