using System;
using System.Configuration;
using Jumpcity.Configuration.SMS;

namespace Jumpcity.Configuration
{
    /// <summary>
    /// 提供对于框架自定义的配置节点信息的访问
    /// </summary>
    public class ConfigurationManagers
    {
        /// <summary>
        /// 获取对于短信功能模块的配置信息
        /// </summary>
        public static SmsSection SmsSection = ConfigurationManager.GetSection("jumpcity.modules.sms") as SmsSection;
    }
}
