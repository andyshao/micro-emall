using System;
using System.Configuration;

namespace Jumpcity.Configuration.SMS
{
    /// <summary>
    /// 提供针对短信功能模块的配置节点信息，无法继承此类
    /// </summary>
    public sealed class SmsSection : ConfigurationSection
    {
        /// <summary>
        /// 获取短信基本配置信息
        /// </summary>
        [ConfigurationProperty("settings")]
        public SmsSettings Settings
        {
            get { return base["settings"] as SmsSettings; }
        }

        /// <summary>
        /// 获取短信发送方法的相关配置信息
        /// </summary>
        [ConfigurationProperty("sendFunction")]
        public SmsSendFunction SendFunction
        {
            get { return base["sendFunction"] as SmsSendFunction; }
        }
    }
}
