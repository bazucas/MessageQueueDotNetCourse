using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    public class MessageElement : ConfigurationElement
    {
        [ConfigurationProperty(SettingName.Name)]
        public string Name
        {
            get { return (string)this[SettingName.Name]; }
        }

        [ConfigurationProperty(SettingName.MessageQueueName)]
        public string MessageQueueName
        {
            get { return (string)this[SettingName.MessageQueueName]; }
        }

        private struct SettingName
        {
            public const string Name = "name";
            public const string MessageQueueName = "messageQueueName";
        }
    }
}
