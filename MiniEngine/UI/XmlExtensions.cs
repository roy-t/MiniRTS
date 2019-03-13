using System.Xml;

namespace MiniEngine.UI
{
    public static class XmlExtensions
    {
        public static bool ReadElementAsBoolean(this XmlReader reader, string element)
        {
            reader.ReadNext(element);
            return reader.ReadElementContentAsBoolean();
        }

        public static int ReadElementAsInt(this XmlReader reader, string element)
        {
            reader.ReadNext(element);
            return reader.ReadElementContentAsInt();
        }

        public static bool ReadNext(this XmlReader reader, string element)
        {
            if(reader.Name.Equals(element, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return reader.ReadToFollowing(element);
        }

        public static void WriteElement(this XmlWriter writer, string element, bool value)
        {
            writer.WriteStartElement(element);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteElement(this XmlWriter writer, string element, int value)
        {
            writer.WriteStartElement(element);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
    }
}
