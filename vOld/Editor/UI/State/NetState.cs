using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MiniEngine.UI.Utilities;

namespace MiniEngine.UI.State
{
    public sealed class NetState : IXmlSerializable
    {
        public bool ShowNetWindow;
        public bool AutoStartServer;
        public bool AutoStartClient;

        public void ReadXml(XmlReader reader)
        {
            this.ShowNetWindow = reader.ReadElementAsBoolean(nameof(this.ShowNetWindow));
            this.AutoStartServer = reader.ReadElementAsBoolean(nameof(this.AutoStartServer));
            this.AutoStartClient = reader.ReadElementAsBoolean(nameof(this.AutoStartClient));

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElement(nameof(this.ShowNetWindow), this.ShowNetWindow);
            writer.WriteElement(nameof(this.AutoStartServer), this.AutoStartServer);
            writer.WriteElement(nameof(this.AutoStartClient), this.AutoStartClient);
        }

        public XmlSchema GetSchema() => null;
    }
}
