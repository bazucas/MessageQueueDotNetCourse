using System.Configuration;

namespace Sixeyed.MessageQueue.Messaging.Configuration
{
    public class PropertyElement : ConfigurationElement
    {
        [ConfigurationProperty(SettingName.Name)]
        public string Name
        {
            get { return (string) this[SettingName.Name]; }
        }

        [ConfigurationProperty(SettingName.Value)]
        public string Value
        {
            get { return (string) this[SettingName.Value]; }
        }

        [ConfigurationProperty(SettingName.Include, DefaultValue=true)]
        public bool Include
        {
            get { return (bool) this[SettingName.Include]; }
        }

        private struct SettingName
        {
            public const string Name = "name";
            public const string Value = "value";
            public const string Include = "include";
        }
    }
}
