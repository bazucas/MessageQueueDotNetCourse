using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    [ConfigurationCollection(typeof(PropertyElement),
        AddItemName = "message",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class MessageCollection : ConfigurationElementCollection<MessageElement>
    {
        protected override string ElementName
        {
            get { return "message"; }
        }

        protected override object GetElementKey(MessageElement element)
        {
            return element.Name;
        }
    }
}
