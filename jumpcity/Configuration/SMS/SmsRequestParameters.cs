using System;
using System.Configuration;

namespace Jumpcity.Configuration.SMS
{
    /// <summary>
    /// 发送短信时所需提交的附加参数
    /// </summary>
    [Serializable]
    public class SmsRequestParameter : ConfigurationElement
    {
        /// <summary>
        /// 获取或设置附加参数的名称
        /// </summary>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// 获取或设置附加参数的默认值
        /// </summary>
        [ConfigurationProperty("value", IsRequired = false)]
        public string DefaultValue
        {
            get { return this["value"] as string; }
            set { this["value"] = value; }
        }
    }
}
