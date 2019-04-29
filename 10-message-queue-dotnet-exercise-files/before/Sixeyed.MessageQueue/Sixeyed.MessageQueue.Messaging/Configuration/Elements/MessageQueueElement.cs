using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    public class MessageQueueElement : ConfigurationElement
    {
        [ConfigurationProperty(SettingName.Name)]
        public string Name
        {
            get { return (string)this[SettingName.Name]; }
        }

        [ConfigurationProperty(SettingName.Type)]
        public string Type
        {
            get { return (string)this[SettingName.Type]; }
        }

        [ConfigurationProperty(SettingName.Properties)]
        public PropertyCollection Properties
        {
            get { return (PropertyCollection)this[SettingName.Properties]; }
        }

        [ConfigurationProperty(SettingName.Queues)]
        public QueueCollection Queues
        {
            get { return (QueueCollection)this[SettingName.Queues]; }
        }

        private struct SettingName
        {
            public const string Name = "name";
            public const string Type = "type";
            public const string Properties = "properties";
            public const string Queues = "queues";
        }
    }
}