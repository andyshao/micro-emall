using System;
using System.Configuration;

namespace Jumpcity.Configuration.SMS
{ 
    public class SmsSendFunction : ConfigurationElement
    {
        [ConfigurationProperty("functionName", IsKey = true, IsRequired = true)]
        public string FunctionName
        {
            get { return this["functionName"] as string; }
            set { this["functionName"] = value; }
        }

        [ConfigurationProperty("method", IsRequired = false, DefaultValue = "GET")]
        public string Method
        {
            get { return this["method"] as string; }
            set { this["method"] = value; }
        }

        [ConfigurationProperty("userName", IsRequired = true)]
        public string UserNameParameterName
        {
            get { return this["userName"] as string; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("passwordName", IsRequired = true)]
        public string PasswordParameterName
        {
            get { return this["passwordName"] as string; }
            set { this["passwordName"] = value; }
        }

        [ConfigurationProperty("mobileName", IsRequired = true)]
        public string MobileParameterName
        {
            get{return this["mobileName"] as string;}
            set{this["mobileName"] = value;}
        }

        [ConfigurationProperty("contentName", IsRequired = true)]
        public string ContentParameterName
        {
            get{return this["contentName"] as string;}
            set{this["contentName"] = value;}
        }

        [ConfigurationProperty("requestParameters")]
        public SmsRequestParameterCollection Parameters
        {
            get { return base["requestParameters"] as SmsRequestParameterCollection; }
        }

        [ConfigurationCollection(typeof(SmsRequestParameter), AddItemName = "add")]
        public class SmsRequestParameterCollection : ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new SmsRequestParameter();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((SmsRequestParameter)element).Name;
            } 

            public SmsRequestParameter this[int index]
            {
                get { return base.BaseGet(index) as SmsRequestParameter; }
            }

            public new SmsRequestParameter this[string name]
            {
                get { return base.BaseGet(name) as SmsRequestParameter; }
            }
        }
    }
}
