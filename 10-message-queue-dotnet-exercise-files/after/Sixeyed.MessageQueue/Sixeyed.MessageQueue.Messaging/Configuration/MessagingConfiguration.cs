
using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    public class MessagingConfiguration : ConfigurationSection
    {
        public static MessagingConfiguration Current
        {
            get { return ConfigurationManager.GetSection("sixeyed.messageQueue.messaging") as MessagingConfiguration; }
        }

        [ConfigurationProperty(SettingName.DefaultMessageQueueName)]
        public string DefaultMessageQueueName
        {
            get { return (string)this[SettingName.DefaultMessageQueueName]; }
        }

        [ConfigurationProperty(SettingName.MessageQueues)]
        public MessageQueueCollection MessageQueues
        {
            get { return (MessageQueueCollection) this[SettingName.MessageQueues]; }
        }

        [ConfigurationProperty(SettingName.Messages)]
        public MessageCollection Messages
        {
            get { return (MessageCollection)this[SettingName.Messages]; }
        }

        private struct SettingName
        {
            public const string DefaultMessageQueueName = "defaultMessageQueueName";
            public const string MessageQueues = "messageQueues";
            public const string Messages = "messages";
            
        }
    }
}