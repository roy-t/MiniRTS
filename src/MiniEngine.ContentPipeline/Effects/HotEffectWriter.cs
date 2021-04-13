using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace MiniEngine.ContentPipeline.Effects
{
    [ContentTypeWriter]
    internal sealed class HotEffectWriter : ContentTypeWriter<HotEffectContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform) =>
            typeof(HotEffectReader).AssemblyQualifiedName;

        protected override void Write(ContentWriter output, HotEffectContent value)
        {
            var bytes = value.GetEffectCode();

            output.Write(value.SourceFile);
            output.Write(bytes.Length);
            output.Write(bytes);
        }
    }
}
