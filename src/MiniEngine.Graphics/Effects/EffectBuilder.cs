using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using Serilog;

namespace MiniEngine.Graphics.Effects
{
    internal sealed class MyLogger : ContentBuildLogger
    {
        private readonly ILogger Logger;

        public MyLogger(ILogger logger)
        {
            this.Logger = logger;
        }

        public override void LogImportantMessage(string message, params object[] messageArgs)
            => this.Logger.Error(string.Format(message, messageArgs));

        public override void LogMessage(string message, params object[] messageArgs)
            => this.Logger.Information(string.Format(message, messageArgs));

        public override void LogWarning(string helpLink, ContentIdentity contentIdentity, string message, params object[] messageArgs)
            => this.Logger.Warning(string.Format(message, messageArgs));
    }

    internal sealed class ProcessorContext : ContentProcessorContext
    {
        private readonly ContentBuildLogger ContentLogger;
        private readonly OpaqueDataDictionary Dictionary;

        private readonly string Path;

        public ProcessorContext(ILogger logger, string path)
        {
            this.ContentLogger = new MyLogger(logger);
            this.Dictionary = new OpaqueDataDictionary();
            this.Path = path;
        }

        public override string BuildConfiguration => string.Empty;
        public override string IntermediateDirectory => string.Empty;
        public override ContentBuildLogger Logger => this.ContentLogger;
        public override ContentIdentity SourceIdentity => new ContentIdentity(this.Path);
        public override string OutputDirectory => string.Empty;
        public override string OutputFilename => string.Empty;
        public override OpaqueDataDictionary Parameters => this.Dictionary;
        public override TargetPlatform TargetPlatform => TargetPlatform.Windows;
        public override GraphicsProfile TargetProfile => GraphicsProfile.HiDef;

        public override void AddDependency(string filename) { }
        public override void AddOutputFile(string filename) { }
        public override TOutput BuildAndLoadAsset<TInput, TOutput>(ExternalReference<TInput> sourceAsset, string processorName, OpaqueDataDictionary processorParameters, string importerName) => throw new NotImplementedException();
        public override ExternalReference<TOutput> BuildAsset<TInput, TOutput>(ExternalReference<TInput> sourceAsset, string processorName, OpaqueDataDictionary processorParameters, string importerName, string assetName) => throw new NotImplementedException();
        public override TOutput Convert<TInput, TOutput>(TInput input, string processorName, OpaqueDataDictionary processorParameters) => throw new NotImplementedException();
    }

    public sealed class EffectBuilder
    {
        private readonly ILogger Logger;
        private readonly Regex EffectCompilerPattern;

        public EffectBuilder(ILogger logger)
        {
            this.Logger = logger;
            this.EffectCompilerPattern = new Regex("\\((.*?)\\):(.*)", RegexOptions.Multiline);
        }

        public Effect? Build(GraphicsDevice device, string path)
        {
            var content = new EffectContent
            {
                Identity = new ContentIdentity(path),
                EffectCode = File.ReadAllText(path)
            };

            var processor = new EffectProcessor();

            try
            {
                var compiled = processor.Process(content, new ProcessorContext(this.Logger, path));
                return new Effect(device, compiled.GetEffectCode());
            }
            catch (InvalidContentException cex)
            {
                var match = this.EffectCompilerPattern.Match(cex.Message);
                if (match.Success)
                {
                    var line = match.Captures[0].Value;
                    var message = $"At {line}";
                    this.Logger.Error($"Failed to reload {path}\n\n{message}");
                }
                else
                {
                    this.Logger.Error(cex, "Failed to reload {@file}", path);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Failed to reload {@file}", path);
            }

            return null;
        }
    }
}
