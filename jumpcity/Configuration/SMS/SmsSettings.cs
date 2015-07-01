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
        /// <summary>
        /// 获取或设置短信服务平台所提供的服务主机地址
        /// </summary>
        [ConfigurationProperty("host", IsRequired = true)]
        public string Host
        {
            get { return this["host"] as string; }
            set { this["host"] = value; }
        }

        /// <summary>
        /// 获取或设置批量发送短信时所支持的接收手机号码最大值
        /// </summary>
        [ConfigurationProperty("maxMobileLength", IsRequired = false)]
        public int MaxMobileLength
        {
            get 
            {
                object length = this["maxMobileLength"];
                return length != null ? (int)length : 0;
            }
            set { this["maxMobileLength"] = value; }
        }

        /// <summary>
        /// 获取或设置发送短信内容所支持的最大字节数
        /// </summary>
        [ConfigurationProperty("maxContentLength", IsRequired = false)]
        public int MaxContentLength
        {
            get
            {
                object length = this["maxContentLength"];
                return length != null ? (int)length : 0;
            }
            set { this["maxContentLength"] = value; }
        }

        /// <summary>
        /// 获取或设置发送短信所需的签名
        /// </summary>
        [ConfigurationProperty("sign", IsRequired = false)]
        public string Sign
        {
            get { return this["sign"] as string; }
            set { this["sign"] = value; }
        }
    }
}
