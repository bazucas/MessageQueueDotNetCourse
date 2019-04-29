using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    [ConfigurationCollection(typeof(QueueElement),
        AddItemName = "messageQueue",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class MessageQueueCollection : ConfigurationElementCollection<MessageQueueElement>
    {
        protected override string ElementName
        {
            get { return "messageQueue"; }
        }

        protected override object GetElementKey(MessageQueueElement element)
        {
            return element.Name;
        }
    }
}
