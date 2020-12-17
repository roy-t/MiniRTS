using Microsoft.Xna.Framework.Content;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline.Serialization.Readers
{
    internal class GeometryModelReader : ContentTypeReader<GeometryModel>
    {
        protected override GeometryModel Read(ContentReader input, GeometryModel _)
            => Read(input);

        public static GeometryModel Read(ContentReader input)
        {
            var model = new GeometryModel();

            var count = input.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                model.Add(GeometryMeshReader.Read(input));
            }

            return model;
        }
    }
}
