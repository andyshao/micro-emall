using System;
using System.Configuration;
using System.Text;

namespace Jumpcity.Configuration.SMS
{
    /// <summary>
    /// 提供短信功能模块的基本设置选项，无法继承此类
    /// </summary>
    [Serializable]
    public sealed class SmsSettings : ConfigurationElement
    {
        [ConfigurationProperty("userName", IsRequired = true)]
        public string UserName
        {
            get { return this["userName"] as string; }
            set { this["userName"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string Password
        {
            get { return this["password"] as string; }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return this["host"] as string; }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("maxMobileLength", IsRequired = false)]
        [IntegerValidator(MinValue = 1)]
        public int MaxMobileLength
        {
            get 
            {
                object length = this["maxMobileLength"];
                return length != null ? (int)length : 0;
            }
            set { this["maxMobileLength"] = value; }
        }

        [ConfigurationProperty("maxContentLength", IsRequired = false)]
        [IntegerValidator(MinValue = 1)]
        public int MaxContentLength
        {
            get
            {
                object length = this["maxContentLength"];
                return length != null ? (int)length : 0;
            }
            set { this["maxContentLength"] = value; }
        }

        [ConfigurationProperty("encoding", IsRequired = false)]
        public Encoding ContentEncoding
        {
            get
            {
                string encoding = this["encoding"] as string;
                return encoding != null ? Encoding.GetEncoding(encoding) : Encoding.Default;
            }
            set { this["encoding"] = value; }
        }

        [ConfigurationProperty("sign", IsRequired = false)]
        public string Sign
        {
            get { return this["sign"] as string; }
            set { this["sign"] = value; }
        }
    }
}
