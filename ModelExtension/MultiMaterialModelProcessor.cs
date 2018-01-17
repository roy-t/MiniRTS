using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace ModelExtension
{
    [ContentProcessor(DisplayName = "ModelExtension :: Multi Material Model Processor")]
    public class MultiMaterialModelProcessor : ModelProcessor
    {
        private static readonly IList<string> AcceptableVertexChannelNames =
            new[]
            {
                VertexChannelNames.TextureCoordinate(0),
                VertexChannelNames.Normal(0),
                VertexChannelNames.Binormal(0),
                VertexChannelNames.Tangent(0),
            };

        [DisplayName("Process Textures")]
        [Description("Searches for a Material Description file (.mat) and processes the related textures")]
        [DefaultValue(true)]
        public virtual bool ProcessTextures { get; set; } = true;

        [DisplayName("Specular Map Fallback")]
        public string SpecularMapFallback { get; set; } = "null_specular.tga";

        [DisplayName("Normal Map Fallback")]
        public string NormalMapFallback { get; set; } = "null_normal.tga";

        [DisplayName("Effect File")]
        [Description("The effect file which shaders to apply to the model")]
        public string EffectFile { get; set; } = "RenderEffect.fx";

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (this.ProcessTextures)
            {
                // Look up all materials and link them to the model
                MaterialLinker.Bind(input, Path.GetFullPath(this.NormalMapFallback), Path.GetFullPath(this.SpecularMapFallback));
            }

            return base.Process(input, context);
        }

        protected override void ProcessVertexChannel(
            GeometryContent geometry,
            int vertexChannelIndex,
            ContentProcessorContext context)
        {
            var vertexChannelName =
                geometry.Vertices.Channels[vertexChannelIndex].Name;

            // if this vertex channel has an acceptable names, process it as normal.
            if (AcceptableVertexChannelNames.Contains(vertexChannelName))
            {
                base.ProcessVertexChannel(geometry, vertexChannelIndex, context);
            }
            // otherwise, remove it from the vertex channels; it's just extra data
            // we don't need.
            else
            {
                geometry.Vertices.Channels.Remove(vertexChannelName);
            }
        }
       
        protected override MaterialContent ConvertMaterial(
            MaterialContent material,
            ContentProcessorContext context)
        {
            var parameters = new OpaqueDataDictionary
            {
                {"ColorKeyColor", this.ColorKeyColor},
                {"ColorKeyEnabled", this.ColorKeyEnabled},
                {"GenerateMipmaps", this.GenerateMipmaps},
                {"PremultiplyTextureAlpha", this.PremultiplyTextureAlpha},
                {"ResizeTexturesToPowerOfTwo", this.ResizeTexturesToPowerOfTwo},
                {"TextureFormat", this.TextureFormat},
                {"DefaultEffect", this.DefaultEffect}
            };

            var deferredShadingMaterial =
                new EffectMaterialContent { Effect = new ExternalReference<EffectContent>(this.EffectFile) };

            // copy the textures in the original material to the new normal mapping
            // material, if they are relevant to our renderer. The
            // LookUpTextures function has added the normal map and specular map
            // textures to the Textures collection, so that will be copied as well.
            foreach (var texture
                in material.Textures)
            {
                if ((texture.Key == "Texture") ||
                    (texture.Key == "NormalMap") ||
                    (texture.Key == "SpecularMap"))
                    deferredShadingMaterial.Textures.Add(texture.Key, texture.Value);
            }

            return context.Convert<MaterialContent, MaterialContent>(
                deferredShadingMaterial,
                typeof(MaterialProcessor).Name, parameters);
        }
      
        #region Hide irrelevant model processor properties
        [Browsable(false)]
        public override bool GenerateTangentFrames => true;

        [Browsable(false)]
        public override Color ColorKeyColor => Color.Magenta;

        [Browsable(false)]
        public override bool ColorKeyEnabled => false;

        [Browsable(false)]
        public override MaterialProcessorDefaultEffect DefaultEffect => MaterialProcessorDefaultEffect.BasicEffect;

        [Browsable(false)]
        public override bool GenerateMipmaps => true;

        [Browsable(false)]
        public override bool PremultiplyTextureAlpha => true;

        [Browsable(false)]
        public override bool PremultiplyVertexColors => true;

        [Browsable(false)]
        public override bool ResizeTexturesToPowerOfTwo => false;

        [Browsable(false)]
        public override TextureProcessorOutputFormat TextureFormat => TextureProcessorOutputFormat.Compressed;
        #endregion
    }
}
