using MiniEngine.Systems;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MiniEngine.UI
{
    public sealed class EntityState : IXmlSerializable
    {
        public bool ShowEntityWindow;        
        public Entity SelectedEntity;
        
        public void ReadXml(XmlReader reader)
        {
            this.ShowEntityWindow = reader.ReadElementAsBoolean(nameof(this.ShowEntityWindow));
            this.SelectedEntity = new Entity(reader.ReadElementAsInt(nameof(this.SelectedEntity)));

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElement(nameof(this.ShowEntityWindow), this.ShowEntityWindow);
            writer.WriteElement(nameof(this.SelectedEntity), this.SelectedEntity.Id);            
        }

        public XmlSchema GetSchema() => null;
    }
}
