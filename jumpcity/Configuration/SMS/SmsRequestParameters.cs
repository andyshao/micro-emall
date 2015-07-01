using System;
using System.Configuration;

namespace Jumpcity.Configuration.SMS
{
    [Serializable]
    public class SmsRequestParameter : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = false)]
        public string DefaultValue
        {
            get { return this["value"] as string; }
            set { this["value"] = value; }
        }
    }
}
