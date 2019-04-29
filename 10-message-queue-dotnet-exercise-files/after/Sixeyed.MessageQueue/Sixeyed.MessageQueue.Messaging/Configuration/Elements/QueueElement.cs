using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    public class QueueElement : ConfigurationElement
    {
        [ConfigurationProperty(SettingName.Address)]
        public string Address
        {
            get { return (string)this[SettingName.Address]; }
        }

        [ConfigurationProperty(SettingName.Name)]
        public string Name
        {
            get { return (string)this[SettingName.Name]; }
        }

        [ConfigurationProperty(SettingName.IsTemporary)]
        public bool IsTemporary
        {
            get { return (bool)this[SettingName.IsTemporary]; }
        }

        private struct SettingName
        {
            public const string Name = "name";
            public const string Address = "address";
            public const string IsTemporary = "isTemporary";
        }
    }
}
