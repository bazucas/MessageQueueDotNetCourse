using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    [ConfigurationCollection(typeof(PropertyElement),
        AddItemName = "property",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class PropertyCollection : ConfigurationElementCollection<PropertyElement>
    {
        protected override string ElementName
        {
            get { return "property"; }
        }

        protected override object GetElementKey(PropertyElement element)
        {
            return element.Name;
        }
    }
}
