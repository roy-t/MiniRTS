using System;
using System.Xml;

namespace MiniEngine.UI
{
    public static class XmlExtensions
    {
        public static T ReadElementAsEnum<T>(this XmlReader reader, string element)
            where T : Enum
        {
            var text = ReadElementAsString(reader, element);

            return (T)Enum.Parse(typeof(T), text);
        }

        public static string ReadElementAsString(this XmlReader reader, string element)
        {
            reader.ReadNext(element);
            return reader.ReadElementContentAsString();
        }

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

        public static void WriteElement(this XmlWriter writer, string element, string value)
        {
            writer.WriteStartElement(element);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }

        public static void WriteElement<T>(this XmlWriter writer, string element, T value)
            where T : Enum
        {
            var text = Enum.GetName(typeof(T), value);
            WriteElement(writer, element, text);
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
