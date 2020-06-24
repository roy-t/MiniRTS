using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MiniEngine.UI.Utilities;

namespace MiniEngine.UI.State
{
    public sealed class NetState : IXmlSerializable
    {
        public bool ShowNetWindow;

        public void ReadXml(XmlReader reader)
        {
            this.ShowNetWindow = reader.ReadElementAsBoolean(nameof(this.ShowNetWindow));

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElement(nameof(this.ShowNetWindow), this.ShowNetWindow);
        }

        public XmlSchema GetSchema() => null;
    }
}
