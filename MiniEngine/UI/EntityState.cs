using MiniEngine.Systems;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MiniEngine.UI
{
    public sealed class EntityState
    {
        public bool ShowEntityWindow;

        [XmlIgnore]
        public Entity SelectedEntity;

        // TODO: fix serialization without messy hidden data!
        //#region SerializationProperties

        //[XmlAttribute(AttributeName = "SelectedEntity")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //public int SelectedEntityInt
        //{
        //    get => this.SelectedEntity;
        //    set => this.SelectedEntity = new Entity(value);
        //}

        //#endregion
    }
}
