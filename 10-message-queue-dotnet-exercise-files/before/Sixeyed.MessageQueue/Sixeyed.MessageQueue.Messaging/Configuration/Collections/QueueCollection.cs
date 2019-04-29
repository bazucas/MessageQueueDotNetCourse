using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    [ConfigurationCollection(typeof(QueueElement),
        AddItemName = "queue",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class QueueCollection : ConfigurationElementCollection<QueueElement>
    {
        protected override string ElementName
        {
            get { return "queue"; }
        }

        protected override object GetElementKey(QueueElement element)
        {
            return element.Name;
        }
    }
}
