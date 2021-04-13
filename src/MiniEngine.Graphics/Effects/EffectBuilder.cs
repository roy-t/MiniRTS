using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Effects
{
    class MyLogger : ContentBuildLogger
    {
        public override void LogImportantMessage(string message, params object[] messageArgs) { }
        public override void LogMessage(string message, params object[] messageArgs) { }
        public override void LogWarning(string helpLink, ContentIdentity contentIdentity, string message, params object[] messageArgs) { }
    }

    class ImporterContext : ContentImporterContext
    {
        private readonly ContentBuildLogger ContentLogger = new MyLogger();

        public override string IntermediateDirectory => string.Empty;
        public override ContentBuildLogger Logger => this.ContentLogger;
        public override string OutputDirectory => string.Empty;
        public override void AddDependency(string filename) { }
    }

    class ProcessorContext : ContentProcessorContext
    {
        private readonly ContentBuildLogger ContentLogger = new MyLogger();
        private readonly OpaqueDataDictionary Dictionary = new OpaqueDataDictionary();

        public override string BuildConfiguration => string.Empty;
        public override string IntermediateDirectory => string.Empty;
        public override ContentBuildLogger Logger => this.ContentLogger;
        public override ContentIdentity SourceIdentity => new ContentIdentity("SourceFile.fx");
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
        public Effect Build(GraphicsDevice device, string path)
        {
            var content = new EffectContent
            {
                Identity = new ContentIdentity(path),
                EffectCode = File.ReadAllText(path)
            };

            var processor = new EffectProcessor();
            var compiled = processor.Process(content, new ProcessorContext());

            return new Effect(device, compiled.GetEffectCode());
        }
    }
}
