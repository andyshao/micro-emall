using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jumpcity.Net;
using Jumpcity.Utility.Extend;
using Jumpcity.Configuration;
using Jumpcity.Configuration.SMS;

namespace Jumpcity.SMS
{
    /// <summary>
    /// 为短信发送服务提供基础实现
    /// </summary>
    public class SmsAdapter
    {
        #region 成员变量...

        private SmsSettings _settings = null;
        private SmsSendFunction _sendFunction = null;
        private string _host = null;
        private string _username = null;
        private string _password = null;
        private Encoding _encoding = Encoding.UTF8;
        private int _maxMobileLength = 50;
        private int _maxContentLength = 140;
        private string _sign = string.Empty;

        #endregion 成员变量...

        #region 构造方法...

        /// <summary>
        /// 创建短信发送服务的实例
        /// </summary>
        public SmsAdapter()
        {
            SmsSection section = ConfigurationManagers.SmsSection;

            if (section != null)
            {
                this._settings = section.Settings;
                this._sendFunction = section.SendFunction;

                if (_settings != null)
                {
                    if (_settings.MaxMobileLength > 0)
                        _maxMobileLength = _settings.MaxMobileLength;
                    if (_settings.MaxContentLength > 0)
                        _maxContentLength = _settings.MaxContentLength;
                }
            }
        }

        #endregion 构造方法...

        #region 成员属性...

        /// <summary>
        /// 获取短信发送功能的基本设置选项
        /// </summary>
        public SmsSettings Settings
        {
            get { return _settings; }
        }

        /// <summary>
        /// 获取短信发送方法的设置选项
        /// </summary>
        public SmsSendFunction SendFunction
        {
            get { return _sendFunction; }
        }

        /// <summary>
        /// 获取或设置短信服务平台提供的主机地址
        /// </summary>
        public string Host
        {
            get 
            {
                if (General.IsNullable(_host))
                {
                    if(_settings != null)
                        _host = _settings.Host;
                }
                return _host;
            }
            set { _host = value; }
        }

        /// <summary>
        /// 获取或设置服务的用户名
        /// </summary>
        public string UserName
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        /// 获取或设置服务的密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// 获取或设置短信内容所使用的编码类型
        /// </summary>
        public Encoding ContentEncoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        /// <summary>
        /// 获取或设置批量发送时可以支持的手机号码最大数量
        /// </summary>
        public int MaxMobileLength
        {
            get { return _maxMobileLength; }
            set { _maxMobileLength = value; }
        }

        /// <summary>
        /// 获取或设置短信内容文本的最大字节数
        /// </summary>
        public int MaxContentLength
        {
            get { return _maxContentLength; }
            set { _maxContentLength = value; }
        }

        /// <summary>
        /// 获取或设置短信发送时要附带的签名信息
        /// </summary>
        public string Sign
        {
            get
            {
                if (General.IsNullable(_sign))
                {
                    if (_settings.Sign != null)
                        _sign = _settings.Sign;
                }
                return _sign;
            }
            set { _sign = value; }
        }

        #endregion 成员属性...

        #region 成员方法...

        /// <summary>
        /// 将要发送的短信内容根据最大字节数设置进行删减，并根据指定的编码类型进行编码
        /// </summary>
        /// <param name="content">要处理的短信内容</param>
        /// <returns>返回处理后的短信内容文本</returns>
        protected virtual string EncodeContent(string content)
        {
            if (!General.IsNullable(content))
            {
                content = content.Trim();
                int maxLength = this.MaxContentLength;
                string sign = this.Sign;

                if (content.Length > maxLength)
                    content = content.Remove(maxLength - sign.Length - 1);

                if (!General.IsNullable(sign))
                    content += sign;

                return HttpUtility.UrlEncode(content, this.ContentEncoding);
            }

            return null;
        }

        /// <summary>
        /// 检测提交的手机号列表，去除无效的号码，之后如果数量超出设置的最大限制数量，则进行删减
        /// </summary>
        /// <param name="numbers">要处理的手机号码列表</param>
        /// <returns>返回处理后的手机号码列表</returns>
        protected virtual List<string> FilterNumbers(List<string> numbers)
        {
            if (!General.IsNullable(numbers))
                numbers = numbers.FindAll(n => General.IsMobile(n)).Take(this.MaxMobileLength).ToList();
            return numbers;
        }

        /// <summary>
        /// 批量发送短信
        /// </summary>
        /// <param name="mobileNumbers">接收短信内容的手机号码列表</param>
        /// <param name="content">短信的内容</param>
        /// <param name="userName">调用服务所需的用户名</param>
        /// <param name="password">调用服务所需的密码</param>
        /// <param name="otherParameters">调用服务所需提交的其它参数</param>
        /// <returns>返回服务器响应的文本内容</returns>
        public virtual string Send(List<string> mobileNumbers, string content, string userName, string password, Dictionary<string, string> otherParameters = null)
        {
            mobileNumbers = this.FilterNumbers(mobileNumbers);
            content = this.EncodeContent(content);
            
            if (
                General.IsNullable(mobileNumbers)
             || General.IsNullable(content)
             || General.IsNullable(userName)
             || General.IsNullable(password)
            )
                return null;

            NetClient cilent = new NetClient();
            string method = this._sendFunction.Method;
            string requestUri = this.Host + this._sendFunction.FunctionName;
            SmsSendFunction.SmsRequestParameterCollection parameters = this._sendFunction.Parameters;

            string request = string.Format(
                "{0}={1}&{2}={3}&{4}={5}&{6}={7}",
                this._sendFunction.UserNameParameterName,
                userName,
                this._sendFunction.PasswordParameterName,
                HttpUtility.UrlEncode(password, this._encoding),
                this._sendFunction.MobileParameterName,
                string.Join(",", mobileNumbers),
                this._sendFunction.ContentParameterName,
                content
            );

            if (!General.IsNullable(parameters))
            {
                bool newValue = !General.IsNullable(otherParameters);

                for (int i = 0, c = parameters.Count; i < c; i++)
                {
                    SmsRequestParameter p = parameters[i];
                    string name = p.Name;
                    string value = p.DefaultValue;
                    if (newValue && otherParameters.ContainsKey(name))
                        value = otherParameters[name];

                    if (!General.IsNullable(value))
                        request += string.Format("&{0}={1}", name, value);
                }
            }

            return cilent.Submit(requestUri, method, this.ContentEncoding, request);
        }
        
        /// <summary>
        /// 批量发送短信
        /// </summary>
        /// <param name="mobileNumbers">接收短信内容的手机号码列表</param>
        /// <param name="content">短信的内容</param>
        /// <param name="otherParameters">调用服务所需提交的其它参数</param>
        /// <returns>返回服务器响应的文本内容</returns>
        public string Send(List<string> mobileNumbers, string content, Dictionary<string, string> otherParameters = null)
        {
            return Send(mobileNumbers, content, this.UserName, this.Password, otherParameters);
        }
        
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobileNumber">接收短信内容的手机号码</param>
        /// <param name="content">短信的内容</param>
        /// <param name="userName">调用服务所需的用户名</param>
        /// <param name="password">调用服务所需的密码</param>
        /// <param name="otherParameters">调用服务所需提交的其它参数</param>
        /// <returns>返回服务器响应的文本内容</returns>
        public string Send(string mobileNumber, string content, string userName, string password, Dictionary<string, string> otherParameters = null)
        {
            return Send(new List<string> { mobileNumber }, content, userName, password, otherParameters);
        }
        
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobileNumber">接收短信内容的手机号码</param>
        /// <param name="content">短信的内容</param>
        /// <param name="otherParameters">调用服务所需提交的其它参数</param>
        /// <returns>返回服务器响应的文本内容</returns>
        public string Send(string mobileNumber, string content, Dictionary<string, string> otherParameters = null)
        {
            return Send(mobileNumber, content, this.UserName, this.Password, otherParameters);
        }

        #endregion 成员方法...
    }
}
