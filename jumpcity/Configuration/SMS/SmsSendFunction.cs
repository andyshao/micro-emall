using System;
using System.Configuration;

namespace Jumpcity.Configuration.SMS
{
    /// <summary>
    /// 发送短信方法的设置选项
    /// </summary>
    [Serializable]
    public class SmsSendFunction : ConfigurationElement
    {
        /// <summary>
        /// 获取或设置短信服务平台对于短信发送方法所提供的API名称
        /// </summary>
        [ConfigurationProperty("functionName", IsKey = true, IsRequired = true)]
        public string FunctionName
        {
            get { return this["functionName"] as string; }
            set { this["functionName"] = value; }
        }

        /// <summary>
        /// 获取或设置短信发送服务所使用的提交类型
        /// </summary>
        [ConfigurationProperty("method", IsRequired = false, DefaultValue = "GET")]
        public string Method
        {
            get { return this["method"] as string; }
            set { this["method"] = value; }
        }

        /// <summary>
        /// 获取或设置调用短信发送服务所需提交的用户名参数名称
        /// </summary>
        [ConfigurationProperty("userName", IsRequired = true)]
        public string UserNameParameterName
        {
            get { return this["userName"] as string; }
            set { this["userName"] = value; }
        }

        /// <summary>
        /// 获取或设置调用短信发送服务所需提交的密码参数名称
        /// </summary>
        [ConfigurationProperty("passwordName", IsRequired = true)]
        public string PasswordParameterName
        {
            get { return this["passwordName"] as string; }
            set { this["passwordName"] = value; }
        }

        /// <summary>
        /// 获取或设置调用短信发送服务所需提交的接收信息手机号的参数名称
        /// </summary>
        [ConfigurationProperty("mobileName", IsRequired = true)]
        public string MobileParameterName
        {
            get{return this["mobileName"] as string;}
            set{this["mobileName"] = value;}
        }

        /// <summary>
        /// 获取或设置调用短信发送服务所需提交的短信内容的参数名称
        /// </summary>
        [ConfigurationProperty("contentName", IsRequired = true)]
        public string ContentParameterName
        {
            get{return this["contentName"] as string;}
            set{this["contentName"] = value;}
        }

        /// <summary>
        /// 获取调用短信发送服务所需的附加参数列表
        /// </summary>
        [ConfigurationProperty("requestParameters")]
        public SmsRequestParameterCollection Parameters
        {
            get { return base["requestParameters"] as SmsRequestParameterCollection; }
        }

        /// <summary>
        /// 附加参数设置选项列表
        /// </summary>
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

            /// <summary>
            /// 根据索引获取指定的附加参数信息
            /// </summary>
            /// <param name="index">要获取参数信息的索引</param>
            /// <returns>返回指定索引的附加参数信息</returns>
            public SmsRequestParameter this[int index]
            {
                get { return base.BaseGet(index) as SmsRequestParameter; }
            }

            /// <summary>
            /// 根据名称获取指定的附加参数信息
            /// </summary>
            /// <param name="name">要获取参数信息的名称</param>
            /// <returns>返回指定名称的附加参数信息</returns>
            public new SmsRequestParameter this[string name]
            {
                get { return base.BaseGet(name) as SmsRequestParameter; }
            }
        }
    }
}
